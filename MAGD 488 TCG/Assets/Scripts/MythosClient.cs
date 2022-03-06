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

public class MythosClient : MonoBehaviour
{
    public static readonly string[] StringSeparators = { "\r\n" };
    private const string k_GlobalIp = "127.0.0.1"; //Server ip
    private const int k_Port = 2552; //port
    private Socket connection;
    [SerializeField] RelayAllocUtp relay;
    public Text user;
    [SerializeField] private Text pass;

    public static MythosClient instance;

    private bool start = false;
    private string code = "";
    private bool codeIn = false;
    private bool waitingForResponse = false;

    void Awake()
    {
        instance = this;
    }
    void Start()
    {
        DontDestroyOnLoad(this);
        Thread thread = new Thread(new ThreadStart(Client));
        thread.Start();
    }
    void Update()
    {
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
            waitingForResponse = false;
        } else if (messageArgArr[0].Equals("logingood", StringComparison.OrdinalIgnoreCase) || messageArgArr[0].Equals("creationgood", StringComparison.OrdinalIgnoreCase)) {
            waitingForResponse = false;
        }
    }
    public void SendCode(string join) //Called when host is done connected to relay, sends join code to server
    {
        Debug.Log("Sent Join Code: " + join);
        int s = connection.Send(Encoding.ASCII.GetBytes("code\r\n" + join + "\r\n"));
    }
    private void OnApplicationQuit()
    {
        connection.Send(Encoding.ASCII.GetBytes("quit\r\n"));
        connection.Shutdown(SocketShutdown.Both);
    }

    public void OnMatchMake()
    {
        Debug.Log("Sent Matchmaking Request");
        connection.Send(Encoding.ASCII.GetBytes("matchmake\r\n"));
    }

    public void OnLogin()
    {
        if (!waitingForResponse)
        {
            Debug.Log("Sent Login Request");
            connection.Send(Encoding.ASCII.GetBytes("login\r\n" + user.text + "\r\npassword\r\n" + pass.text));
            waitingForResponse = true;
        }
    }

    public void OnCreateAccount()
    {
        if (!waitingForResponse) {
            Debug.Log("Sent Account Creation Request");
            connection.Send(Encoding.ASCII.GetBytes("newaccount\r\n" + user.text + "\r\npassword\r\n" + pass.text));
            waitingForResponse = true;
        }
        
    }
}
