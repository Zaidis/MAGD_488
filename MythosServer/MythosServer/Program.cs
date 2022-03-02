using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;

#nullable enable

namespace MythosServer {
    class Program {
        class User {
            public Socket Socket;
            public string Username;
            public int Skill;
        }
        private const string KGlobalIp = "127.0.0.1"; //External IP
        private const string KLocalIp = "127.0.0.1"; //Local IP
        private const int KPort = 2552; //Port selected
        private static List<Socket> _connections = new List<Socket>(); //List of all connections, and list of matchmaking clients
        private static List<User> _users = new List<User>();
        private static List<User> _matchmaking = new List<User>();

        static void Main(string[] args) {
            Server();
        }
        private static void Server() {
            IPAddress ipAddress = IPAddress.Parse(KLocalIp);
            IPEndPoint localEp = new IPEndPoint(ipAddress, KPort);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEp); //Bind to local ip and port
            listener.Listen(1);

            Console.WriteLine("Waiting...");

            for (; ; ) { //Welcome loop/matchmaking loop
                try {
                    Socket handler = listener.Accept(); //Accept incoming client connection requests, create new socket and thread
                    _connections.Add(handler);

                    Thread thread = new Thread(new ThreadStart(() => ClientHandler(handler)));
                    thread.Start();

                    PrintConnections();
                } catch (Exception e) {
                    Console.Write(e);
                }
                if (_matchmaking.Count > 1) { //matchmake if users matchmaking > 1
                    byte[] buffer = new byte[1024];

                    Socket host = _matchmaking[0].Socket; //Skill matching will go here
                    Socket client = _matchmaking[1].Socket;

                    host.Send(Encoding.ASCII.GetBytes("start\r\n")); //Send start command to selected host
                    Console.WriteLine("Start command sent to host");

                    //SPLIT INTO DIFFERENT THREAD HERE??
                    int numBytesReceived = host.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    Console.WriteLine("Recieved Message: " + textReceived);

                    if (textReceived.IndexOf("code\r\n") == 0) {
                        client.Send(Encoding.ASCII.GetBytes("connect\r\n" + textReceived.Substring(6, numBytesReceived - 6)));
                        Console.WriteLine("Sent connection message to client");
                    }

                    _matchmaking.Remove(_matchmaking.First(user => user.Socket == host));
                    _matchmaking.Remove(_matchmaking.First(user => user.Socket == client));
                }
            }
        }
        private static void ClientHandler(Socket handler) //Handle client until handed off to matchmaking
        {
            byte[] buffer = new byte[1024];
            string[] stringSeparators = new string[] { "\r\n" };
            for (; ; )
            {
                if (_matchmaking.All(user => user.Socket != handler)) {
                    int numBytesReceived = handler.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    Console.WriteLine("Received: \n" + textReceived);
                    string[] messageArgArr = textReceived.Split(stringSeparators, StringSplitOptions.None);
                    
                    if (messageArgArr[0].Equals("matchmake", StringComparison.OrdinalIgnoreCase)) { //Adding connection to matchmaking queue
                        if (_users.Any(user => user.Socket == handler)) { //checking if client is logged in
                            User current = _users.First(user => user.Socket == handler);
                            if (!_matchmaking.Contains(current)) {
                                _matchmaking.Add(current); //add struct which contains needed info to matchmaking list
                                PrintConnections();
                            }
                        }
                    } else if (messageArgArr[0].Equals("login", StringComparison.OrdinalIgnoreCase)) {
                        User? newUser = Login(messageArgArr[1], messageArgArr[2], handler);
                        if (newUser != null)
                            _users.Add((User)newUser);
                    } else if (messageArgArr[0].Equals("newaccount", StringComparison.OrdinalIgnoreCase)) {
                        Console.WriteLine(NewUser(messageArgArr[1], messageArgArr[2]) ? "Creation Successful!" : "Creation Unsuccessful"); //also send message to client
                    } else if (messageArgArr[0].Equals("quit", StringComparison.OrdinalIgnoreCase)) { //Exit case
                        _connections.Remove(handler);
                        PrintConnections();
                        break;
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
            foreach (Socket s in _connections)
                Console.WriteLine(s.RemoteEndPoint);
            if (_users.Count > 0) {
                Console.WriteLine("Logged in Clients: ");
                foreach (User u in _users)
                    if (u.Socket.RemoteEndPoint != null)
                        Console.WriteLine(u.Socket.RemoteEndPoint + " : " + u.Username + " : Skill : " + u.Skill);
            }
            if (_matchmaking.Count > 0) {
                Console.WriteLine("Matchmaking Clients: ");
                foreach (User u in _matchmaking)
                    if (u.Socket.RemoteEndPoint != null)
                        Console.WriteLine(u.Socket.RemoteEndPoint + " : " + u.Username + " : Skill : " + u.Skill);
            }
        }
        private static bool NewUser(string username, string password) {
            using (SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db")) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT EXISTS(SELECT 1 FROM User WHERE Username = $Username) ";
                command.Parameters.AddWithValue("$Username", username);
                using (SqliteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read())
                        if (reader.GetInt32(0) == 1) {
                            connection.Close();
                            return false; //if Username exists, return false, else continue
                        }
                }
                byte[] salt = new byte[128 / 8];

                using (var rngCsp = new RNGCryptoServiceProvider())
                    rngCsp.GetNonZeroBytes(salt);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));
                command.CommandText = @"INSET INTO User (Username, Salt, Hash) VALUES ($Username, $Salt, $Hash)";
                command.Parameters.AddWithValue("$Username", username);
                command.Parameters.AddWithValue("$Salt", salt);
                command.Parameters.AddWithValue("$Hash", hashed);
                command.ExecuteNonQuery();
                connection.Close();
                return true; //after user has been created return true
            }
        }
        private static User? Login(string username, string password, Socket socket)
        {
            using (SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db"))
            {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT u.Username, u.Salt, u.Hash, s.Skill FROM User u, Skill s WHERE Username = $Username";
                command.Parameters.AddWithValue("$Username", username);
                string parsedUsername = "";
                User user = new User();
                user.Username = username;
                user.Socket = socket;
                byte[] salt = new byte[128/8];
                string hash = "";
                using (SqliteDataReader reader = command.ExecuteReader()) {
                    while (reader.Read()) {
                        parsedUsername = (string)reader["u.Username"];
                        if (parsedUsername.Equals("")) {
                            connection.Close();
                            return null;
                        }
                        salt = Encoding.ASCII.GetBytes((string)reader["u.Salt"]);
                        hash = (string)reader["u.Hash"];
                        user.Skill = (int)reader["s.Skill"];
                    }
                }         

                using (var rngCsp = new RNGCryptoServiceProvider())
                    rngCsp.GetNonZeroBytes(salt);

                string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                    password: password,
                    salt: salt,
                    prf: KeyDerivationPrf.HMACSHA256,
                    iterationCount: 100000,
                    numBytesRequested: 256 / 8));

                if (hash.Equals(hashed)) {
                    connection.Close();
                    return user;
                }
            }
            return null;
        }
    }
}