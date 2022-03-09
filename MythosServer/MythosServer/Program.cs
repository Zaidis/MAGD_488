﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.Data.Sqlite;

#nullable enable

namespace MythosServer {
    class Program {
        public static readonly string[] StringSeparators = { "\r\n" };
        class User 
        {
            public readonly string Username;
            public readonly int Skill;
            public Match? Match = null;
            public User(string u, int sk) { Username = u; Skill = sk; }
        }
        class Match 
        {
            public Socket Host;
            public Socket Client;
            public string Hostoutcome = "";
            public string Clientoutcome = "";
            public Match(Socket h, Socket c) { Host = h; Client = c; }
        }

        private const string KLocalIp = "127.0.0.1"; //Local IP
        private const int KPort = 2552; //Port selected
        private static readonly List<Socket> Connections = new List<Socket>(); //List of all connections, and list of matchmaking clients
        private static readonly List<User> Users = new List<User>();
        private static readonly List<Match> Matches = new List<Match>();
        private static List<Socket> _matchmaking = new List<Socket>();
        private static readonly Dictionary<Socket, User> UserSocketDictionary = new Dictionary<Socket, User>();
        private static Mutex sqlLock = new Mutex();

        static void Main() {
            IPAddress ipAddress = IPAddress.Parse(KLocalIp);
            IPEndPoint localEp = new IPEndPoint(ipAddress, KPort);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEp); //Bind to local ip and port
            listener.Listen(1);

            Console.WriteLine("Waiting...");

            Thread mmthread = new Thread(() => Matchmaking());
            mmthread.Start();

            for (; ; ) { //Welcome loop
                try {
                    Socket handler = listener.Accept(); //Accept incoming client connection requests, create new socket and thread
                    Connections.Add(handler);

                    Thread thread = new Thread(() => ClientHandler(handler));
                    thread.Start();

                    PrintConnections();
                } catch (Exception e) { Console.Write(e); }
            }
        }
        private static void ClientHandler(Socket handler) //Handle client communication, run in thread
        {
            byte[] buffer = new byte[1024];
            bool loggedIn = false;
            for (; ; ) {
                Thread.Sleep(1);
                if (!_matchmaking.Contains(handler)) {
                    int numBytesReceived = handler.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);

                    if (messageArgArr[0].Equals("matchmake", StringComparison.OrdinalIgnoreCase)) { //Adding connection to matchmaking queue
                        if (UserSocketDictionary.ContainsKey(handler)) { //checking if client is logged in
                            if (!_matchmaking.Contains(handler)) {
                                _matchmaking.Add(handler); //add struct which contains needed info to matchmaking list
                                PrintConnections();
                            }
                        }
                    } else if (messageArgArr[0].Equals("login", StringComparison.OrdinalIgnoreCase)) {
                        User? newUserLogin = Login(messageArgArr[1], messageArgArr[2], handler);
                        if (newUserLogin != null) {
                            handler.Send(Encoding.ASCII.GetBytes("logingood\r\n"));
                            Users.Add(newUserLogin);
                            UserSocketDictionary.Add(handler, newUserLogin);
                            loggedIn = true;
                        } else
                            handler.Send(Encoding.ASCII.GetBytes("loginbad\r\n"));
                        PrintConnections();
                    } else if (messageArgArr[0].Equals("newaccount", StringComparison.OrdinalIgnoreCase)) {
                        handler.Send(NewUser(messageArgArr[1], messageArgArr[2]) ? Encoding.ASCII.GetBytes("creationgood\r\n") : Encoding.ASCII.GetBytes("creationbad\r\n"));
                        PrintConnections();
                    } else if(messageArgArr[0].Equals("outcome", StringComparison.OrdinalIgnoreCase)) {
                        if(loggedIn)
                            MatchOutcome(messageArgArr[1], handler);
                    } else if (messageArgArr[0].Equals("getdecknames", StringComparison.OrdinalIgnoreCase)) {
                        if(loggedIn)
                            GetDeckNames(handler);
                    } else if (messageArgArr[0].Equals("getdeckcontent", StringComparison.OrdinalIgnoreCase)) {
                        if (loggedIn)
                            GetDeckContent(handler, messageArgArr[1]);
                    } else if (messageArgArr[0].Equals("quit", StringComparison.OrdinalIgnoreCase)) { //Exit case
                        Connections.Remove(handler);
                        _matchmaking.Remove(handler);
                        Users.Remove(UserSocketDictionary[handler]);
                        UserSocketDictionary.Remove(handler);
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
            foreach (Socket s in Connections)
                Console.WriteLine(s.RemoteEndPoint);
            if (Users.Count > 0) {
                Console.WriteLine("Logged in Clients: ");
                foreach (var sock in UserSocketDictionary)
                    Console.WriteLine(sock.Key.RemoteEndPoint + " : " + sock.Value.Username + " : Skill : " + sock.Value.Skill);
            }
            if (_matchmaking.Count > 0) {
                Console.WriteLine("Matchmaking Clients: ");
                foreach (Socket s in _matchmaking)
                    Console.WriteLine(s.RemoteEndPoint + " : " + UserSocketDictionary[s].Username + " : Skill : " + UserSocketDictionary[s].Skill);
            }
        }
        private static bool NewUser(string username, string password) //Attempt to create a user based on passed username and password, return true for success, return false for failure
        {
            sqlLock.WaitOne();
            Console.WriteLine("Entered User Creation");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT EXISTS(SELECT 1 FROM User WHERE Username = @us) ";
            command.Parameters.AddWithValue("@us", username);
            using (SqliteDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    if (reader.GetInt32(0) == 1) {
                        connection.Close();
                        sqlLock.ReleaseMutex();
                        return false; //if Username exists, return false, else continue
                    }
                }
            }

            byte[] salt = new byte[128 / 8];
            using (var rngCsp = new RNGCryptoServiceProvider())
                rngCsp.GetNonZeroBytes(salt);
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));

            command.CommandText = @"INSERT INTO User (Username, Salt, Hash) " + "VALUES (@u, @s, @h)";
            command.Parameters.AddWithValue("@u", username);
            command.Parameters.AddWithValue("@s", salt);
            command.Parameters.AddWithValue("@h", hashed);
            command.ExecuteNonQuery();
            connection.Close(); //bug in adding paramters
            sqlLock.ReleaseMutex();
            return true; //after user has been created return true
        }
        private static void Matchmaking() //Function responsible for matchmaking loop, runs continuously, if at least 2 users are in the queue, match closest two in skill, add to a match, and remove from pool
        {
            for(; ; ) {
                Thread.Sleep(1);
                if (_matchmaking.Count > 1) { //matchmake if users matchmaking > 1
                    byte[] buffer = new byte[1024];
                    int minDifference = int.MaxValue;
                    Socket host = _matchmaking[0], client = _matchmaking[1];
                    _matchmaking = _matchmaking.OrderBy(s => UserSocketDictionary[s].Skill).ToList();
                    for (int i = 2; i < _matchmaking.Count; i++) { //find two users with closest skill, match together
                        int currentDifference = Math.Abs(UserSocketDictionary[_matchmaking[i]].Skill - UserSocketDictionary[_matchmaking[i]].Skill);
                        if (currentDifference < minDifference) {
                            minDifference = currentDifference;
                            host = _matchmaking[i];
                            client = _matchmaking[i - 1];
                        }
                    }

                    host.Send(Encoding.ASCII.GetBytes("start\r\n")); //Send start command to selected host
                    Console.WriteLine("Start command sent to host");

                    int numBytesReceived = host.Receive(buffer); //data stream in
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
                    Console.WriteLine("Received Message: " + textReceived);

                    if (messageArgArr[0].Equals("code")) {
                        client.Send(Encoding.ASCII.GetBytes("connect\r\n" + messageArgArr[1]));
                        Console.WriteLine("Sent connection message to client");
                    }

                    Match newMatch = new Match(host, client);
                    Matches.Add(newMatch);

                    UserSocketDictionary[host].Match = newMatch;
                    UserSocketDictionary[client].Match = newMatch;

                    _matchmaking.Remove(host);
                    _matchmaking.Remove(client);
                }
            }            
        }
        private static void MatchOutcome(string outcome, Socket comm) //set outcome of match of passed User according to if socket is host or client in match, then eval
        {
            if (UserSocketDictionary[comm].Match != null) {
                Match match = UserSocketDictionary[comm].Match!;
                if (comm == match.Host)
                    match.Hostoutcome = outcome;
                else
                    match.Clientoutcome = outcome;
                EvaluateOutcome(match);
            }
        }
        private static void EvaluateOutcome(Match match)  //evaulate outcome of a match, if outcomes match change skill accordingly, if not, change nothing
        {
            if (match.Hostoutcome.Equals(match.Clientoutcome)) {
                sqlLock.WaitOne();
                //Code that changes skill of users based on win/loss, and stores it with sqlite
                sqlLock.ReleaseMutex();                
            } else if (!match.Hostoutcome.Equals("") && !match.Clientoutcome.Equals(""))
                Console.Write("u1 and u2 outcome do not match no skill will be changed!");
        }
        private static User? Login(string username, string password, Socket socket)  //login "socket" based on passed username and password, create User and return it
        {
            sqlLock.WaitOne();
            Console.WriteLine("Entered User Login");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT u.Username, u.Salt, u.Hash FROM User u WHERE u.Username = @u";
            command.Parameters.AddWithValue("@u", username);
            User? user = null;
            byte[] salt = new byte[128/8];
            string hash = "";
            using (SqliteDataReader reader = command.ExecuteReader()) {
                while (reader.Read()) {
                    string parsedUsername = reader.GetString(0);
                    if (parsedUsername.Equals("")) {
                        connection.Close();
                        sqlLock.ReleaseMutex();
                        return null;
                    }
                    reader.GetBytes(1, 0, salt, 0, 16);
                    hash = reader.GetString(2);
                    user = new User(username, 1500/*(int)reader["s.Skill"]*/);
                }
            }
            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2( password, salt, KeyDerivationPrf.HMACSHA256, 100000, 256 / 8));

            if (hash.Equals(hashed)) {
                connection.Close();
                if (Users.All(u => u.Username != username))
                    return user;
                Console.WriteLine("Already Logged In!");
                sqlLock.ReleaseMutex();

            }

            return null;
        }
        private static void GetDeckNames(Socket sock)
        {
            Console.WriteLine("Entered Deck Name Retrieval");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT d.Deckname FROM Deck d, User u WHERE @us = u.Username AND u.Username = d.Username";
            command.Parameters.AddWithValue("@us", UserSocketDictionary[sock].Username);
            String message = "decknames\r\n";
            using (SqliteDataReader reader = command.ExecuteReader())
                while (reader.Read())
                    message = message + reader.GetString(0) + "\r\n";
            sock.Send(Encoding.ASCII.GetBytes(message));
            connection.Close();
        }
        private static void GetDeckContent(Socket sock, string deckname) {
            Console.WriteLine("Entered Deck Content Retrieval");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            connection.Open();
            SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"SELECT d.Deck FROM Deck d, User u WHERE @us = u.Username AND u.Username = d.Username";
            command.Parameters.AddWithValue("@us", UserSocketDictionary[sock].Username);
            byte[] cards = new byte[40*2]; //using uint16
            using (SqliteDataReader reader = command.ExecuteReader())
                while (reader.Read())
                    reader.GetBytes(0, 0, cards, 0, 40*2);
            sock.Send(Encoding.ASCII.GetBytes("deckcontent\r\n").Concat(cards).ToArray()); //concats cards stored as uint16 byte stream to message
            connection.Close();
        }
    }
}