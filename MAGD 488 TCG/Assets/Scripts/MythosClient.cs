using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class MythosClient : MonoBehaviour {
    public static MythosClient instance; //Singleton

    public static readonly string[] StringSeparators = { "\r\n" };
    private const string k_GlobalIp = "127.0.0.1"; //Server ip
    private const int k_Port = 2552; //port
    private Socket connection;
    [SerializeField] RelayAllocUtp relay;
    public Text user;
    [SerializeField] private Text pass;
    public List<string> deckNames { get; private set; }
    public List<int> currentDeck { get; private set; }
    private bool start = false;
    private string code = "";
    private bool codeIn = false;

    void Awake() {
        DontDestroyOnLoad(this);
        if (instance != null && instance != this)
            Destroy(this);
        else
            instance = this;
    }
    void Start()
    {
        deckNames = new List<string>();
        currentDeck = new List<int>();
        Thread thread = new Thread(new ThreadStart(Client));
        thread.Start();
    }
    void Update() {
        if (start) { //Find a better way to sync thread?
            relay.OnHost();
            start = false;
        }
        if (codeIn) {
            Debug.Log("Received Code:" + code);
            relay.OnJoin(code);
            codeIn = false;
        }
    }
    private void Client() //Start threaded, connect to server, receive one message from server, either start as host, or connect
    {
        IPAddress ipAddress = IPAddress.Parse(k_GlobalIp);
        IPEndPoint remoteEp = new IPEndPoint(IPAddress.Parse(k_GlobalIp), k_Port);
        connection = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp); //Create socket
        connection.Connect(remoteEp); //Connect to server
        Debug.Log("Connected to " + connection.RemoteEndPoint);

        byte[] buffer = new byte[1024]; //buffer for incoming data
        int numBytesReceived = connection.Receive(buffer); //data stream in
        string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
        string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
        Debug.Log("Received String: " + textReceived);
        if (messageArgArr[0].Equals("start", StringComparison.OrdinalIgnoreCase)) {
            start = true;
        } else if (messageArgArr[0].Equals("connect", StringComparison.OrdinalIgnoreCase)) {
            code = messageArgArr[1];
            codeIn = true;
        } else if (messageArgArr[0].Equals("loginbad", StringComparison.OrdinalIgnoreCase) || messageArgArr[0].Equals("creationbad", StringComparison.OrdinalIgnoreCase)) {
            //Display Failed Login Message
        } else if (messageArgArr[0].Equals("logingood", StringComparison.OrdinalIgnoreCase) || messageArgArr[0].Equals("creationgood", StringComparison.OrdinalIgnoreCase)) {
            //Load next scene do something, good login
        } else if (messageArgArr[0].Equals("decknames", StringComparison.OrdinalIgnoreCase)) {
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
    public void SendCode(string join) //Called when host is done connected to relay, sends join code to server
    {
        Debug.Log("Sent Join Code: " + join);
        int s = connection.Send(Encoding.ASCII.GetBytes("code\r\n" + join + "\r\n"));
    }
    private void OnApplicationQuit() {
        connection.Send(Encoding.ASCII.GetBytes("quit\r\n"));
        connection.Shutdown(SocketShutdown.Both);
    }
    public void OnMatchMake() {
        Debug.Log("Sent Matchmaking Request");
        connection.Send(Encoding.ASCII.GetBytes("matchmake\r\n"));
    }
    public void OnLogin() {
        Debug.Log("Sent Login Request");
        connection.Send(Encoding.ASCII.GetBytes("login\r\n" + user.text + "\r\npassword\r\n" + pass.text));
    }
    public void OnCreateAccount() {
        Debug.Log("Sent Account Creation Request");
        connection.Send(Encoding.ASCII.GetBytes("newaccount\r\n" + user.text + "\r\npassword\r\n" + pass.text));
    }
    public void OnRetrieveDeckNames() {
        Debug.Log("Sent Deck Names Request");
        connection.Send(Encoding.ASCII.GetBytes("getdecknames\r\n"));
    }
    public void OnRetrieveDeckContent(string name) {
        Debug.Log("Sent Deck Content Request");
        connection.Send(Encoding.ASCII.GetBytes("getdeckcontent\r\n" + name));
    }

    public void OnSaveDeck(string name, int[] cards)
    {
        string message = "savedeck\r\n" + name + "\r\n";
        foreach (int i in cards)
            message += i + ",";
        message = message.TrimEnd(',');
        connection.Send(Encoding.ASCII.GetBytes(message));
    }

    public void OnOutcome(bool outcome) //takes in bool, true for hostvictory, false for clientvictory
    {
        Debug.Log("Sent Outcome Message");
        connection.Send(Encoding.ASCII.GetBytes("outcome\r\n" + (outcome ? "hostvictory" : "clientvictory")));
    } 
}
