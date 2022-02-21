using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading;

namespace MythosServer
{
    class Program
    {
        private const string k_GlobalIp = "127.0.0.1"; //External IP
        private const string k_LocalIp = "127.0.0.1"; //Local IP
        private const int k_Port = 2552; //Port selected
        private static List<Socket> connections = new List<Socket>();
        private static List<Socket> matchmaking = new List<Socket>();

        static void Main(string[] args)
        {
            Server();
        }

        private static void Server()
        {
            IPAddress ipAddress = IPAddress.Parse(k_LocalIp);
            IPEndPoint localEp = new IPEndPoint(ipAddress, k_Port);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEp); //Bind to local ip and port
            listener.Listen(1);

            Console.WriteLine("Waiting...");

            Thread mmthread = new Thread(new ThreadStart(() => MatchMake()));
            mmthread.Start();

            for (; ; )
            {
                try
                {
                    Socket handler = listener.Accept(); //Accept incoming client connection requests, create new socket and thread
                    connections.Add(handler);

                    Thread thread = new Thread(new ThreadStart(() => ClientHandler(handler)));
                    thread.Start();

                    PrintConnections();
                }
                catch (Exception e)
                {
                    Console.Write(e);
                }
            }
        }
        private static void ClientHandler(Socket handler)
        {
            byte[] buffer = new byte[1024];

            for (; ; )
            {
                if (!matchmaking.Contains(handler))
                {
                    Thread.Sleep(10); //reduce frequency of loop, reducing cpu utilization
                    int numBytesReceived = handler.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    Console.WriteLine("Received: " + textReceived);
                    if (textReceived.Equals("quit\r\n", StringComparison.OrdinalIgnoreCase))
                    { //Exit case
                        connections.Remove(handler);
                        matchmaking.Remove(handler);
                        PrintConnections();
                        break;
                    }
                    if (textReceived.Equals("matchmake\r\n", StringComparison.OrdinalIgnoreCase))
                    {
                        matchmaking.Add(handler);
                        PrintConnections();
                    }
                }
            }

            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        private static void PrintConnections()
        {
            Console.Clear();
            Console.WriteLine("Current Connections: ");
            foreach (Socket s in connections)
                Console.WriteLine(s.RemoteEndPoint);
            if (matchmaking.Count > 0)
            {
                Console.WriteLine("Matchmaking Clients: ");
                foreach (Socket s in matchmaking)
                    if (s.RemoteEndPoint != null)
                        Console.WriteLine(s.RemoteEndPoint);
            }
        }

        private static void MatchMake()
        {
            for (; ; )
            {
                if (matchmaking.Count > 1)
                {
                    byte[] buffer = new byte[1024];

                    Socket host = matchmaking[0];
                    Socket client = matchmaking[1];

                    host.Send(Encoding.ASCII.GetBytes("start\r\n"));
                    Console.WriteLine("Start command sent to host");
                    int numBytesReceived = host.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    Console.WriteLine("Recieved Message: " + textReceived);
                    if (textReceived.IndexOf("code\r\n") == 0)
                    {
                        client.Send(Encoding.ASCII.GetBytes("connect\r\n" + textReceived.Substring(6, numBytesReceived - 6)));
                        Console.WriteLine("Sent connection message to client");
                    }
                    matchmaking.Remove(host);
                    matchmaking.Remove(client);
                }
            }
        }
    }
}
