using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Runtime.ExceptionServices;
using System.Threading;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Identity;

namespace MythosServer
{
    class Program
    {
        struct User
        {
            public Socket Socket;
            public string Username;
            public int Skill;
        }
        private const string k_GlobalIp = "127.0.0.1"; //External IP
        private const string k_LocalIp = "127.0.0.1"; //Local IP
        private const int k_Port = 2552; //Port selected
        private static List<Socket> connections = new List<Socket>(); //List of all connections, and list of matchmaking clients
        private static List<User> users = new List<User>();
        private static List<User> matchmaking = new List<User>();

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

            Thread mmthread = new Thread(new ThreadStart(() => MatchMake())); //Start main matchmaking thread
            mmthread.Start();

            for (; ; ) { //Welcome loop
                try {
                    Socket handler = listener.Accept(); //Accept incoming client connection requests, create new socket and thread
                    connections.Add(handler);

                    Thread thread = new Thread(new ThreadStart(() => ClientHandler(handler)));
                    thread.Start();

                    PrintConnections();
                } catch (Exception e) {
                    Console.Write(e);
                }
            }
        }
        private static void ClientHandler(Socket handler) //Handle client until handed off to matchmaking
        {
            byte[] buffer = new byte[1024];

            for (; ; ) {
                Thread.Sleep(10); //reduce frequency of loop, reducing cpu utilization
                if (matchmaking.All(user => user.Socket != handler)) {
                    int numBytesReceived = handler.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    Console.WriteLine("Received: \n" + textReceived);
                    if (textReceived.Equals("quit\r\n", StringComparison.OrdinalIgnoreCase)) { //Exit case
                        connections.Remove(handler);
                        PrintConnections();
                        break;
                    } if (textReceived.Equals("matchmake\r\n", StringComparison.OrdinalIgnoreCase)) { //Adding connection to matchmaking queue
                        //if (users.Any(user => user.Socket == handler)) { //checking if client is logged in
                        if(true){
                            //User current = users.First(user => user.Socket == handler);
                            User current = new User();
                            current.Socket = handler;
                            if (!matchmaking.Contains(current))
                            {
                                matchmaking.Add(current); //add struct which contains needed info to matchmaking list
                                PrintConnections();
                            }
                        }
                    } else if (textReceived.StartsWith("login\r\n")) {
                        int passIndex = textReceived.IndexOf("\r\npassword\r\n");
                        string user = textReceived.Substring(7, passIndex - 1);
                        string pass = textReceived.Substring(passIndex + 13, textReceived.Length);
                        User? newUser = Login(user, pass, handler);
                        if (newUser != null)
                           users.Add((User)newUser);
                    } else if (textReceived.StartsWith("newaccount\r\n")) {
                        int passIndex = textReceived.IndexOf("\r\npassword\r\n");
                        string user = textReceived.Substring(12, passIndex - 1);
                        string pass = textReceived.Substring(passIndex + 13, textReceived.Length);
                        Console.WriteLine(NewUser(pass, user) ? "Creation Successful!" : "Creation Unsuccessful"); //also send message to client
                    }
                }
            }
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        private static void PrintConnections() //Print current connection statuses
        {
            Console.Clear();
            Console.WriteLine("Current Connections: ");
            foreach (Socket s in connections)
                Console.WriteLine(s.RemoteEndPoint);
            if (users.Count > 0)
            {
                Console.WriteLine("Logged in Clients: ");
                foreach (User u in users)
                    if (u.Socket.RemoteEndPoint != null)
                        Console.WriteLine(u.Socket.RemoteEndPoint + " : " + u.Username + " : Skill : " + u.Skill);
            }
            if (matchmaking.Count > 0) {
                Console.WriteLine("Matchmaking Clients: ");
                foreach (User u in matchmaking)
                    if (u.Socket.RemoteEndPoint != null)
                        Console.WriteLine(u.Socket.RemoteEndPoint + " : " + u.Username + " : Skill : " + u.Skill);
            }
        }
        private static void MatchMake()
        {
            for (; ; ) {
                if (matchmaking.Count > 1) {
                    byte[] buffer = new byte[1024];

                    Socket host = matchmaking[0].Socket; //Skill matching will go here
                    Socket client = matchmaking[1].Socket;

                    host.Send(Encoding.ASCII.GetBytes("start\r\n")); //Send start command to selected host
                    Console.WriteLine("Start command sent to host");

                    int numBytesReceived = host.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    Console.WriteLine("Recieved Message: " + textReceived);

                    if (textReceived.IndexOf("code\r\n") == 0) {
                        client.Send(Encoding.ASCII.GetBytes("connect\r\n" + textReceived.Substring(6, numBytesReceived - 6)));
                        Console.WriteLine("Sent connection message to client");
                    }

                    matchmaking.Remove(matchmaking.First(user => user.Socket == host));
                    matchmaking.Remove(matchmaking.First(user => user.Socket == client));
                }
            }
        }
        private static bool NewUser(string Username, string Password)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db"))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT EXISTS(SELECT 1 FROM User WHERE Username = $Username) ";
                command.Parameters.AddWithValue("$Username", Username);
                using (SqliteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read())
                        if (reader.GetInt32(0) == 1)
                            return false; //if Username exists, return false, else continue
                }
                byte[] salt = new byte[128 / 8];

                using (var rngCsp = new RNGCryptoServiceProvider())
                    rngCsp.GetNonZeroBytes(salt);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: Password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                command.CommandText = @"INSET INTO User (Username, Salt, Hash) VALUES ($Username, $Salt, $Hash)";
                command.Parameters.AddWithValue("$Username", Username);
                command.Parameters.AddWithValue("$Salt", salt);
                command.Parameters.AddWithValue("$Hash", hashed);
                command.ExecuteNonQuery();
                return true; //after user has been created return true
            }
        }
        private static User? Login(string Username, string Password, Socket socket) //sqlite access can likely be simplified
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db"))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT EXISTS(SELECT 1 FROM User WHERE Username = $Username) ";
                command.Parameters.AddWithValue("$Username", Username);
                using (SqliteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read())
                        if (reader.GetInt32(0) == 0)
                            return null; //if Username doesn't exist, return null, else continue
                }

                User user = new User();
                bool Authenticated = false;
                byte[] salt = null;
                command.CommandText = @"SELECT Salt FROM User WHERE Username = $Username";
                command.Parameters.AddWithValue("$Username", Username);
                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        salt = Encoding.ASCII.GetBytes(reader.GetString(0));
                if (salt != null) {
                    using (var rngCsp = new RNGCryptoServiceProvider())
                        rngCsp.GetNonZeroBytes(salt);

                    string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                        password: Password,
                        salt: salt,
                        prf: KeyDerivationPrf.HMACSHA256,
                        iterationCount: 100000,
                        numBytesRequested: 256 / 8));
                    command.CommandText = @"SELECT Hash FROM User WHERE Username = $Username";
                    command.Parameters.AddWithValue("$Username", Username);
                    using (SqliteDataReader reader = command.ExecuteReader())
                        while (reader.Read())
                            if (reader.GetString(0) == hashed)
                                Authenticated = true;
                    if (Authenticated)
                    {
                        command.CommandText = @"SELECT Skill FROM Skill WHERE Username = $Username";
                        command.Parameters.AddWithValue("$Username", Username);
                        using (SqliteDataReader reader = command.ExecuteReader())
                            while (reader.Read())
                                user.Skill = reader.GetInt32(0);
                        user.Username = Username;
                        user.Socket = socket;
                        return user;
                    }
                }
            }
            return null;
        }
    }
}