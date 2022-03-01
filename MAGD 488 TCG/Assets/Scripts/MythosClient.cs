using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class MythosClient : MonoBehaviour
{
    private const string k_GlobalIp = "127.0.0.1"; //Server ip
    private const int k_Port = 2552; //port
    private Socket connection;

    [SerializeField] RelayAllocUtp relay;

    public static MythosClient instance;

    private bool start = false;
    private string code = "";
    private bool codeIn = false;

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
        Debug.Log("Received String: " + textReceived);
        if (textReceived.Length > 0) {
            if (textReceived.IndexOf("start\r\n", StringComparison.Ordinal) == 0) {
                start = true;
            } else if (textReceived.IndexOf("connect\r\n", StringComparison.Ordinal) == 0) {
                code = textReceived.Substring(9, 6);
                codeIn = true;
            }
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
}
