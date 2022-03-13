using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;
using UnityEngine;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Button = UnityEngine.UI.Button;

public class MythosClient : MonoBehaviour {
    public static MythosClient instance; //Singleton

    public static readonly string[] StringSeparators = { "\r\n" };
    private const string k_GlobalIp = "127.0.0.1"; //Server ip
    private const int k_Port = 2552; //port
    private Socket connection;

    public TMP_InputField user;
    public TMP_InputField pass;
    public TMP_Text status;
    public Button LoginButton;
    public Button CreateButton;
    public GameObject FailureToConnectPanel;
    public GameObject LoginPanel;
    private bool SuccessfullyConnected = false;
    public GameObject ConnectingPanel;
    private bool ConnecionFailure = false;
    [SerializeField] private string gameScene;
    public List<string> deckNames { get; private set; }
    public List<int> currentDeck { get; private set; }

    private static RSACryptoServiceProvider csp;
    private static RSAParameters pubKey;

    private (string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] key, string joinCode) serverOutcome;
    private bool serverStarted = false;
    private (string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] hostConnectionData, byte[] key) clientOutcome;
    private bool clientStarted = false;
    private bool loginGood = false, loginBad = false, creationGood = false, creationBad = false;
    void Awake() {
        DontDestroyOnLoad(this);
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    async void Start()
    {
        try {
            await UnityServices.InitializeAsync();
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
            var playerID = AuthenticationService.Instance.PlayerId;
        } catch (Exception e) {
            Debug.Log(e);
        }
        deckNames = new List<string>();
        currentDeck = new List<int>();
        new Thread(Client).Start();
    }
    void Update() {
        SyncThread();
    }
    private async void Client() //Start threaded, connect to server, receive one message from server, either start as host, or connect
    {
        IPAddress ipAddress = IPAddress.Parse(k_GlobalIp);
        IPEndPoint remoteEp = new IPEndPoint(IPAddress.Parse(k_GlobalIp), k_Port);
        connection = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Create socket
        try {
            connection.Connect(remoteEp); //Connect to server
        } catch (Exception e) {
            Debug.Log(e);
            ConnecionFailure = true;
            return;
        }
        SuccessfullyConnected = true;
        Debug.Log("Connected to " + connection.RemoteEndPoint);

        byte[] buffer = new byte[1024]; //buffer for incoming data
        for (;;) {
            int numBytesReceived = connection.Receive(buffer); //data stream in
            string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
            string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
            Debug.Log("TEXT RECEIVED: " + textReceived);
            if (messageArgArr[0].Equals("start", StringComparison.OrdinalIgnoreCase)) {
                serverOutcome =  await AllocateRelayServerAndGetJoinCode(2);
                connection.Send(Encoding.ASCII.GetBytes("code\r\n" + serverOutcome.joinCode));
                serverStarted = true;
            } else if (messageArgArr[0].Equals("rsakey", StringComparison.OrdinalIgnoreCase)) {
                string xmlFile = textReceived.Substring(8, textReceived.Length - 8);
                StringReader sr = new StringReader(xmlFile);
                XmlSerializer xs = new XmlSerializer(typeof(RSAParameters));
                pubKey = (RSAParameters)xs.Deserialize(sr);
                csp = new RSACryptoServiceProvider();
                csp.ImportParameters(pubKey);
            } else if (messageArgArr[0].Equals("connect", StringComparison.OrdinalIgnoreCase)) {
                clientOutcome = await JoinRelayServerFromJoinCode(messageArgArr[1]);
                clientStarted = true;
            } else if (messageArgArr[0].Equals("salt", StringComparison.OrdinalIgnoreCase)) {
                byte[] bytesPlainTextData = System.Text.Encoding.Unicode.GetBytes("password\r\n" +
                    Convert.ToBase64String(KeyDerivation.Pbkdf2(pass.text, Convert.FromBase64String(messageArgArr[1]),
                        KeyDerivationPrf.HMACSHA256, 100000, 256 / 8)));
                byte[] bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
                string cypherText = Convert.ToBase64String(bytesCypherText);
                connection.Send(Encoding.ASCII.GetBytes(cypherText));
            } 
            else if (messageArgArr[0].Equals("loginbad", StringComparison.OrdinalIgnoreCase)) { loginBad = true; } 
            else if (messageArgArr[0].Equals("creationbad", StringComparison.OrdinalIgnoreCase)) { creationBad = true; } 
            else if (messageArgArr[0].Equals("logingood", StringComparison.OrdinalIgnoreCase)) { loginGood = true; } 
            else if (messageArgArr[0].Equals("creationgood", StringComparison.OrdinalIgnoreCase)) { creationGood = true; } 
            else if (messageArgArr[0].Equals("decknames", StringComparison.OrdinalIgnoreCase)) {
                deckNames.Clear();
                for (int i = 1; i < messageArgArr.Length; i++)
                    deckNames.Add(messageArgArr[i]);
            } else if (messageArgArr[0].Equals("deckcontent", StringComparison.OrdinalIgnoreCase)) {
                currentDeck.Clear();
                string[] splitIntsAsStrings = messageArgArr[1].Split(',');
                foreach (string intString in splitIntsAsStrings)
                    currentDeck.Add(Convert.ToInt32(intString));
            }
        }
    }

    public void OnStartClientThread() {
        FailureToConnectPanel.SetActive(false);
        new Thread(Client).Start();
    }
    private void OnApplicationQuit() {
        if (!connection.Connected)
            return;
        connection.Send(Encoding.ASCII.GetBytes("quit\r\n"));
        connection.Shutdown(SocketShutdown.Both);
        connection.Close();
    }
    public void OnMatchMake() {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Matchmaking Request");
        connection.Send(Encoding.ASCII.GetBytes("matchmake\r\n"));
    }
    public void OnLogin() {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Login Request");
        connection.Send(Encoding.ASCII.GetBytes("login\r\n" + user.text));
    }
    public void OnCreateAccount() {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Account Creation Request");
        byte[] salt = new byte[128 / 8];
        using (var rngCsp = new RNGCryptoServiceProvider())
            rngCsp.GetNonZeroBytes(salt);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(pass.text, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        connection.Send(Encoding.ASCII.GetBytes("newaccount\r\n" + user.text + "\r\n" + Convert.ToBase64String(salt) + "\r\n" + hash));
    }
    public void OnRetrieveDeckNames() {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Deck Names Request");
        connection.Send(Encoding.ASCII.GetBytes("getdecknames\r\n"));
    }
    public void OnRetrieveDeckContent(string name) {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Deck Content Request");
        connection.Send(Encoding.ASCII.GetBytes("getdeckcontent\r\n" + name));
    }

    public void OnSaveDeck(string name, int[] cards)
    {
        if (!connection.Connected)
            return;
        string message = "savedeck\r\n" + name + "\r\n";
        foreach (int i in cards)
            message += i + ",";
        message = message.TrimEnd(',');
        connection.Send(Encoding.ASCII.GetBytes(message));
    }

    public void OnOutcome(bool outcome) //takes in bool, true for hostvictory, false for clientvictory
    {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Outcome Message");
        connection.Send(Encoding.ASCII.GetBytes("outcome\r\n" + (outcome ? "hostvictory" : "clientvictory")));
    } private void SyncThread() {
        if (ConnecionFailure) {
            FailureToConnectPanel.SetActive(true);
            ConnecionFailure = false;
        } else if (SuccessfullyConnected) {
            ConnectingPanel.SetActive(false);
            LoginPanel.SetActive(true);
        }
        if (serverStarted) {
            var (ipv4address, port, allocationIdBytes, connectionData, key, joinCode) = serverOutcome;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipv4address, port, allocationIdBytes, key, connectionData, true);
            NetworkManager.Singleton.StartHost();
            serverStarted = false;
        } else if (clientStarted) {
            var (ipv4address, port, allocationIdBytes, connectionData, hostConnectionData, key) = clientOutcome;
            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipv4address, port, allocationIdBytes, key, connectionData, hostConnectionData, true);
            NetworkManager.Singleton.StartClient();
            clientStarted = false;
        }
        if (loginGood) {
            status.text = "Login Succeeded!";
            status.color = new Color(0f, 1f, 0f, 1f);
            loginGood = false;
            SceneManager.LoadScene(gameScene);
        } else if (loginBad) {
            pass.text = "";
            user.text = "";
            status.text = "Login Failed!";
            status.color = Color.red;
            status.color = new Color(1f, 0f, 0f, 1f);
            LoginButton.interactable = true;
            loginBad = false;
        } else if (creationGood) {
            status.text = "Account Creation Succeeded!";
            status.color = new Color(0f, 1f, 0f, 1f);
            creationGood = false;
        } else if (creationBad) {
            pass.text = "";
            user.text = "";
            status.text = "Creation Failed!";
            status.color = new Color(1f, 0f, 0f, 1f);
            CreateButton.interactable = true;
            creationBad = false;
        }
    }
    public static async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] key, string joinCode)> AllocateRelayServerAndGetJoinCode(int maxConnections, string region = null) {
        Allocation allocation;
        string createJoinCode;
        try {
            allocation = await Relay.Instance.CreateAllocationAsync(maxConnections, region);
        } catch (Exception e) {
            Debug.LogError($"Relay create allocation request failed {e.Message}");
            throw;
        }

        Debug.Log($"server: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"server: {allocation.AllocationId}");

        try {
            createJoinCode = await Relay.Instance.GetJoinCodeAsync(allocation.AllocationId);
        } catch {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");

        return (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.Key, createJoinCode);
    }
    public static async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] hostConnectionData, byte[] key)> JoinRelayServerFromJoinCode(string joinCode) {
        JoinAllocation allocation;
        try {
            allocation = await Relay.Instance.JoinAllocationAsync(joinCode);
        } catch {
            Debug.LogError("Relay create join code request failed");
            throw;
        }

        Debug.Log($"client: {allocation.ConnectionData[0]} {allocation.ConnectionData[1]}");
        Debug.Log($"host: {allocation.HostConnectionData[0]} {allocation.HostConnectionData[1]}");
        Debug.Log($"client: {allocation.AllocationId}");

        var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");

        return (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.HostConnectionData, allocation.Key);
    }
}
