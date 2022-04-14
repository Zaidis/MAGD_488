using System;
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
using UnityEngine.UI;

public class MythosClient : MonoBehaviour {
    public static MythosClient instance; //Singleton
    private Queue<Action> syncFunctions;    

    public static readonly string[] StringSeparators = { "\r\n" };
    private const string k_GlobalIp = "3.81.142.105"; //Server ip
    //private static string k_GlobalIp = "127.0.0.1"; //server IP
    private const int k_Port = 2552; //port
    IPAddress ipAddress;
    IPEndPoint remoteEp;
    private Socket connection;
    public TMP_InputField user;
    public TMP_InputField pass;
    public string userName;
    public string opponentUserName;
    public TMP_Text status;
    public UnityEngine.UI.Button LoginButton;
    public UnityEngine.UI.Button CreateButton;
    public GameObject FailureToConnectPanel;
    public GameObject LoginPanel;
    public GameObject ConnectingPanel;
    [SerializeField] private string menuScene;
    [SerializeField] private string gameScene;

    public static event Action<List<string>> OnDecknamesLoaded;
    public static event Action<List<int>> OnDeckContentLoaded;
    private List<string> deckNames;
    private List<int> currentDeck;

    private static RSACryptoServiceProvider csp;
    private static RSAParameters pubKey;
    private static byte[] aesKey;

    void Awake() {
        DontDestroyOnLoad(this);
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    async void Start()
    {
        syncFunctions = new Queue<Action>();

        using(Aes InitAes = Aes.Create()) {
            aesKey = InitAes.Key;
        }

        ipAddress = IPAddress.Parse(k_GlobalIp);
        remoteEp = new IPEndPoint(IPAddress.Parse(k_GlobalIp), k_Port);
        connection = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Create socket
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
        while (syncFunctions.Count > 0) {
            Action function = syncFunctions.Dequeue();
            function();
        }
    }
    private async void Client() //Start threaded, connect to server, receive one message from server, either start as host, or connect
    {        
        try {
            connection.Connect(remoteEp); //Connect to server
        } catch (Exception e) {
            Debug.Log(e);
            syncFunctions.Enqueue(()=> FailureToConnectPanel.SetActive(true));
            return;
        }
        syncFunctions.Enqueue(() => {
            ConnectingPanel.SetActive(false);
            LoginPanel.SetActive(true);
        });
        Debug.Log("Connected to " + connection.RemoteEndPoint);

        byte[] buffer = new byte[1024]; //buffer for incoming data
        int numBytesReceived = connection.Receive(buffer);
        string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
        string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
        if (messageArgArr[0].Equals("rsakey", StringComparison.OrdinalIgnoreCase)) { //if the server doesn't greet with an rsa key, disconnect
            Debug.Log("RSA KEY: " + messageArgArr[1]);
            string xmlFile = textReceived.Substring(8, textReceived.Length - 8);
            StringReader sr = new StringReader(xmlFile);
            XmlSerializer xs = new XmlSerializer(typeof(RSAParameters));
            pubKey = (RSAParameters)xs.Deserialize(sr);
            csp = new RSACryptoServiceProvider();
            csp.ImportParameters(pubKey);
            byte[] bytesPlainTextData = Encoding.ASCII.GetBytes("aes\r\n" + Convert.ToBase64String(aesKey));
            byte[] bytesCypherText = csp.Encrypt(bytesPlainTextData, false);
            string cypherText = Convert.ToBase64String(bytesCypherText);
            connection.Send(Encoding.ASCII.GetBytes(cypherText));
        } else
            return;
        for (;;) {
            numBytesReceived = connection.Receive(buffer); //data stream in
            textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
            messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
            textReceived = DecrpytBase64ToString(messageArgArr[1], Convert.FromBase64String(messageArgArr[0]));
            messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
            Debug.Log("TEXT RECEIVED: " + textReceived);
            if (messageArgArr[0].Equals("start", StringComparison.OrdinalIgnoreCase)) {
                var serverOutcome =  await AllocateRelayServerAndGetJoinCode(2);
                connection.Send(EncryptStringToBase64Bytes("code\r\n" + serverOutcome.joinCode));
                opponentUserName = messageArgArr[1];
                syncFunctions.Enqueue(() => {
                    var (ipv4address, port, allocationIdBytes, connectionData, key, joinCode) = serverOutcome;
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipv4address, port, allocationIdBytes, key, connectionData, true);
                    SceneManager.LoadScene(gameScene);                    
                });
            } else if (messageArgArr[0].Equals("connect", StringComparison.OrdinalIgnoreCase)) {
                var clientOutcome = await JoinRelayServerFromJoinCode(messageArgArr[1]);
                opponentUserName = messageArgArr[2];
                syncFunctions.Enqueue(() => {
                    var (ipv4address, port, allocationIdBytes, connectionData, hostConnectionData, key) = clientOutcome;
                    SceneManager.LoadScene(gameScene);
                    NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipv4address, port, allocationIdBytes, key, connectionData, hostConnectionData, true);
                    NetworkManager.Singleton.StartClient();
                });
            } else if (messageArgArr[0].Equals("salt", StringComparison.OrdinalIgnoreCase)) {
                connection.Send(EncryptStringToBase64Bytes("password\r\n" + Convert.ToBase64String(KeyDerivation.Pbkdf2(pass.text, Convert.FromBase64String(messageArgArr[1]), KeyDerivationPrf.HMACSHA256, 100000, 256 / 8))));
            } else if (messageArgArr[0].Equals("logingood", StringComparison.OrdinalIgnoreCase)) {
                syncFunctions.Enqueue(() => {                    
                    userName = user.text;
                    status.text = "Login Succeeded!";
                    status.color = new Color(0f, 1f, 0f, 1f);
                    SceneManager.LoadScene(menuScene);
                });
            } else if (messageArgArr[0].Equals("loginbad", StringComparison.OrdinalIgnoreCase)) {
                syncFunctions.Enqueue(() => {
                    pass.interactable = true;
                    user.interactable = true;
                    pass.text = "";
                    user.text = "";
                    status.text = "Login Failed!";
                    status.color = Color.red;
                    status.color = new Color(1f, 0f, 0f, 1f);
                    LoginButton.interactable = true;
                });
            } else if (messageArgArr[0].Equals("creationgood", StringComparison.OrdinalIgnoreCase)) {
                syncFunctions.Enqueue(() => {
                    status.text = "Account Creation Succeeded!";
                    status.color = new Color(0f, 1f, 0f, 1f);
                });
            } else if (messageArgArr[0].Equals("creationbad", StringComparison.OrdinalIgnoreCase)) {
                syncFunctions.Enqueue(() => {
                    pass.text = "";
                    user.text = "";
                    status.text = "Creation Failed!";
                    status.color = new Color(1f, 0f, 0f, 1f);
                    CreateButton.interactable = true;
                });
            } 
            else if (messageArgArr[0].Equals("decknames", StringComparison.OrdinalIgnoreCase)) {
                deckNames.Clear();
                for (int i = 1; i < messageArgArr.Length; i++)
                    deckNames.Add(messageArgArr[i]);
                syncFunctions.Enqueue(() => {
                    if (OnDecknamesLoaded != null)
                        OnDecknamesLoaded(deckNames);
                });                
            } else if (messageArgArr[0].Equals("deckcontent", StringComparison.OrdinalIgnoreCase)) {
                currentDeck.Clear();
                string[] splitIntsAsStrings = messageArgArr[1].Split(',');
                foreach (string intString in splitIntsAsStrings)
                    currentDeck.Add(Convert.ToInt32(intString));
                syncFunctions.Enqueue(() => {
                    if (OnDeckContentLoaded != null)
                        OnDeckContentLoaded(currentDeck);
                });                
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
        connection.Send(EncryptStringToBase64Bytes("quit\r\n"));
        connection.Shutdown(SocketShutdown.Both);
        connection.Close();
    }
    public void OnMatchMake() {
        if (!connection.Connected)
            return;
        status.text = "Matchmaking...";
        status.color = Color.white;
        Debug.Log("Sent Matchmaking Request");
        connection.Send(EncryptStringToBase64Bytes("matchmake\r\n"));
    }
    public void OnLogin() {
        if (!connection.Connected)
            return;
        pass.interactable = false;
        user.interactable = false;
        status.text = "Attempting Login...";
        status.color = Color.white;
        Debug.Log("Sent Login Request");
        connection.Send(EncryptStringToBase64Bytes("login\r\n" + user.text));
    }
    public void OnCreateAccount() {
        if (!connection.Connected)
            return;
        status.text = "Attempting To Create Account...";
        status.color = Color.white;
        Debug.Log("Sent Account Creation Request");
        byte[] salt = new byte[128 / 8];
        using (var rngCsp = new RNGCryptoServiceProvider())
            rngCsp.GetNonZeroBytes(salt);
        string hash = Convert.ToBase64String(KeyDerivation.Pbkdf2(pass.text, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));
        connection.Send(EncryptStringToBase64Bytes("newaccount\r\n" + user.text + "\r\n" + Convert.ToBase64String(salt) + "\r\n" + hash));
    }
    public void OnRetrieveDeckNames() { //subscribe to OnDecknamesLoaded to get List<string> back
        if (!connection.Connected)
            return;
        Debug.Log("Sent Deck Names Request");
        connection.Send(EncryptStringToBase64Bytes("getdecknames\r\n"));
    }
    public void OnRetrieveDeckContent(string name) { //subscribe to OnDeckContentLoaded to get List<int> back
        if (!connection.Connected)
            return;
        Debug.Log("Sent Deck Content Request");
        connection.Send(EncryptStringToBase64Bytes("getdeckcontent\r\n" + name));
    }

    public void OnSaveDeck(string name, int[] cards)
    {
        if (!connection.Connected)
            return;
        string message = "savedeck\r\n" + name + "\r\n";
        foreach (int i in cards)
            message += i + ",";
        message = message.TrimEnd(',');
        connection.Send(EncryptStringToBase64Bytes(message));
    }

    public void OnOutcome(bool outcome) //takes in bool, true for hostvictory, false for clientvictory
    {
        if (!connection.Connected)
            return;
        Debug.Log("Sent Outcome Message");
        connection.Send(EncryptStringToBase64Bytes("outcome\r\n" + (outcome ? "hostvictory" : "clientvictory")));
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
    private static byte[] EncryptStringToBase64Bytes(string plainText) {
        if (plainText == null || plainText.Length <= 0)
            throw new ArgumentNullException("plainText");
        byte[] encrypted;
        string Base64IV;
        using (Aes aes = Aes.Create()) {
            aes.Key = aesKey;
            Base64IV = Convert.ToBase64String(aes.IV) + "\r\n";
            ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
            using (MemoryStream msEncrypt = new MemoryStream()) {
                using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write)) {
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt)) {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }
        }
        return Encoding.ASCII.GetBytes(Base64IV + Convert.ToBase64String(encrypted));
    }
    private static string DecrpytBase64ToString(string cipherText, byte[] IV) {
        if (cipherText == null || cipherText.Length <= 0)
            throw new ArgumentNullException("cipherText");
        byte[] cipherBytes = Convert.FromBase64String(cipherText);       

        string plaintext = null;
        using (Aes aes = Aes.Create()) {
            aes.Key = aesKey;
            aes.IV = IV;
            ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
            using (MemoryStream msDecrypt = new MemoryStream(cipherBytes)) {
                using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read)) {
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt)) {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }
        }
        return plaintext;
    }
}