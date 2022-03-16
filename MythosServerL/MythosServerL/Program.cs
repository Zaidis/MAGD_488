using System.Data;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Data.Sqlite;

namespace MythosServer {    
    class Program {
        public static readonly string[] StringSeparators = { "\r\n" };  
        private const string KLocalIp = "10.0.3.201"; //Local IP
        private const int KPort = 2552; //Port selected

        private static readonly List<Socket> Connections = new List<Socket>();
        private static readonly List<User> Users = new List<User>();
        private static readonly List<Match> Matches = new List<Match>();
        private static List<Socket> MatchmakingSockets = new List<Socket>();
        private static readonly Dictionary<Socket, User> UserSocketDictionary = new Dictionary<Socket, User>();

        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private static RSAParameters privKey;
        private static RSAParameters pubKey;
        private static string pubKeyString = "";

        private static Object SQLLock = new Object();
        private static Object MatchmakingLock = new Object();
        static void Main() {
            privKey = csp.ExportParameters(true);
            pubKey = csp.ExportParameters(false);
            StringWriter sw = new System.IO.StringWriter();
            XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, pubKey);
            pubKeyString = sw.ToString();

            IPAddress ipAddress = IPAddress.Parse(KLocalIp);
            IPEndPoint localEp = new IPEndPoint(ipAddress, KPort);

            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            listener.Bind(localEp); //Bind to local ip and port
            listener.Listen(1);

            Console.WriteLine("Server Started!");
            for (; ; ) { //Welcome loop
                try {
                    Socket handler = listener.Accept(); //Accept incoming client connection requests, create new socket and thread
                    Thread thread = new Thread(() => ClientHandler(handler));
                    thread.Start();
                } catch (Exception e) { Console.Write(e); }
            }
        }
        private static void ClientHandler(Socket handler) //Handle client communication, run in thread
        {
            Connections.Add(handler);
            byte[] buffer = new byte[1024];
            bool loggedIn = false;
            handler.Send(Encoding.ASCII.GetBytes("rsakey\r\n" + pubKeyString));
            for (; ; ) {
                if (!Connections.Contains(handler) || !handler.Connected)
                    break;
                Thread.Sleep(1);
                PrintConnections();
                int numBytesReceived;
                try {
                    numBytesReceived = handler.Receive(buffer);
                } catch (SocketException e) {
                    Console.WriteLine(e);
                    HandleDisconnect(handler);
                    break;
                }
                string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);

                if (messageArgArr[0].Equals("matchmake", StringComparison.OrdinalIgnoreCase)) { //Adding connection to matchmaking queue
                    if (UserSocketDictionary.ContainsKey(handler)) { //checking if client is logged in
                        if (!MatchmakingSockets.Contains(handler)) {
                            MatchmakingSockets.Add(handler); //add struct which contains needed info to matchmaking list
                            Matchmaking(handler);
                        }
                    }
                } else if (messageArgArr[0].Equals("login", StringComparison.OrdinalIgnoreCase)) {
                    if (!loggedIn) {
                        User? newUserLogin = Login(messageArgArr[1], handler);
                        if (newUserLogin != null) {
                            handler.Send(Encoding.ASCII.GetBytes("logingood\r\n"));
                            Users.Add(newUserLogin);
                            UserSocketDictionary.Add(handler, newUserLogin);
                            loggedIn = true;
                        } else
                            handler.Send(Encoding.ASCII.GetBytes("loginbad\r\n"));
                    }
                } else if (messageArgArr[0].Equals("newaccount", StringComparison.OrdinalIgnoreCase)) {
                    handler.Send(NewUser(messageArgArr[1], messageArgArr[2], messageArgArr[3]) ? Encoding.ASCII.GetBytes("creationgood\r\n") : Encoding.ASCII.GetBytes("creationbad\r\n"));

                } else if (messageArgArr[0].Equals("outcome", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        MatchOutcome(messageArgArr[1], handler);
                } else if (messageArgArr[0].Equals("getdecknames", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        GetDeckNames(handler);
                } else if (messageArgArr[0].Equals("getdeckcontent", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        GetDeckContent(handler, messageArgArr[1]);
                } else if (messageArgArr[0].Equals("savedeck", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        SaveDeckContent(handler, messageArgArr[1], messageArgArr[2]);
                } else if (messageArgArr[0].Equals("quit", StringComparison.OrdinalIgnoreCase)) { //Exit case
                    HandleDisconnect(handler);
                }
            }
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        private static void PrintConnections() //Print current connection statuses
        {
            Console.Clear();
            Console.WriteLine("Current Connections: ");
            foreach (Socket s in Connections.ToList())
                Console.WriteLine(s.RemoteEndPoint);
            if (Users.Count > 0) {
                Console.WriteLine("Logged in Clients: ");
                foreach (var sock in UserSocketDictionary.ToList())
                    Console.WriteLine(sock.Key.RemoteEndPoint + " : " + sock.Value.Username + " : Skill : " + sock.Value.Skill);
            }
            if (MatchmakingSockets.Count > 0) {
                Console.WriteLine("Matchmaking Clients: ");
                foreach (Socket s in MatchmakingSockets.ToList())
                    Console.WriteLine(s.RemoteEndPoint + " : " + UserSocketDictionary[s].Username + " : Skill : " + UserSocketDictionary[s].Skill);
            }
        }
        private static bool NewUser(string username, string salt, string hash) //Attempt to create a user based on passed username and password, return true for success, return false for failure
        {
            lock (SQLLock) {
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
                            return false; //if Username exists, return false, else continue
                        }
                    }
                }
                command.CommandText = @"INSERT INTO User (Username, Salt, Hash) " + "VALUES (@u, @s, @h)";
                command.Parameters.AddWithValue("@u", username);
                command.Parameters.AddWithValue("@s", salt);
                command.Parameters.AddWithValue("@h", hash);
                command.ExecuteNonQuery();
                connection.Close();
                return true; //after user has been created return true
            }
        }
        private static void Matchmaking(Socket handler) //Function responsible for matchmaking loop, runs continuously, if at least 2 users are in the queue, match closest two in skill, add to a match, and remove from pool
        {
            lock (MatchmakingLock) {
                if (MatchmakingSockets.Count > 1 && UserSocketDictionary[handler].Match == null) { //matchmake if users matchmaking > 1
                    Console.WriteLine("Attempting to Match");
                    byte[] buffer = new byte[1024];
                    Socket host = MatchmakingSockets[1];
                    Socket client = MatchmakingSockets[0];
                    int minDifference = int.MaxValue;
                    MatchmakingSockets = MatchmakingSockets.OrderBy(s => UserSocketDictionary[s].Skill).ToList();
                    for (int i = 1; i < MatchmakingSockets.Count; i++) { //find two users with closest skill, match together
                        int currentDifference = Math.Abs(UserSocketDictionary[MatchmakingSockets[i]].Skill - UserSocketDictionary[MatchmakingSockets[i]].Skill);
                        if (currentDifference < minDifference) {
                            minDifference = currentDifference;
                            host = MatchmakingSockets[i];
                            client = MatchmakingSockets[i - 1];
                        }
                    }

                    host.Send(Encoding.ASCII.GetBytes("start\r\n" + UserSocketDictionary[client].Username)); //Send start command to selected host
                    Console.WriteLine("Start command sent to host");
                    int numBytesReceived = 0;
                    try {
                        numBytesReceived = handler.Receive(buffer);
                    } catch (SocketException e) {
                        Console.WriteLine(e);
                        HandleDisconnect(handler);
                        return;
                    }
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
                    Console.WriteLine(textReceived);
                    if (messageArgArr[0].Equals("code")) {
                        Console.WriteLine("Sent " + "connect\r\n" + messageArgArr[1] + "\nto " + client.RemoteEndPoint + " : " + UserSocketDictionary[client].Username + " : Skill : " + UserSocketDictionary[client].Skill);
                        client.Send(Encoding.ASCII.GetBytes("connect\r\n" + messageArgArr[1] + "\r\n" + UserSocketDictionary[host].Username));
                        Console.WriteLine("Sent connection message to client");
                        Match newMatch = new Match(host, client);
                        Matches.Add(newMatch);

                        UserSocketDictionary[host].Match = newMatch;
                        UserSocketDictionary[client].Match = newMatch;

                        MatchmakingSockets.Remove(host);
                        MatchmakingSockets.Remove(client);
                    } else if (messageArgArr[0].Equals("quit", StringComparison.OrdinalIgnoreCase)) //Exit case
                        HandleDisconnect(host);
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
                lock (SQLLock) {
                    //Code that changes skill of users based on win/loss, and stores it with sqlite 
                }
            } else if (!match.Hostoutcome.Equals("") && !match.Clientoutcome.Equals(""))
                Console.Write("u1 and u2 outcome do not match no skill will be changed!");
        }
        private static User? Login(string username, Socket socket)  //login "socket" based on passed username and password, create User and return it
        {
            if (username == "")
                return null;
            byte[] buffer = new byte[1024];
            string salt = "";
            string hash = "";

            Console.WriteLine("Entered User Login");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT Salt, Hash From User WHERE Username=@us";
                command.Parameters.AddWithValue("@us", username);

                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read()) {
                        salt = reader.GetString(0);
                        hash = reader.GetString(1);
                    }
                connection.Close();
            }

            socket.Send(Encoding.ASCII.GetBytes("salt\r\n" + salt));
            int numBytesReceived = 0;
            try {
                numBytesReceived = socket.Receive(buffer);
            } catch (SocketException e) {
                Console.WriteLine(e);
                HandleDisconnect(socket);
                return null;
            }
            string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
            string[] messageArgArr;
            try { //try catch to handle unexepected text that isn't cypertext
                var bytesCypherText = Convert.FromBase64String(textReceived);
                csp.ImportParameters(privKey);
                var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                textReceived = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
                messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
            } catch (Exception e) {
                Console.WriteLine(e);
                Console.WriteLine(textReceived);
                if (textReceived.Equals("quit\r\n"))
                    HandleDisconnect(socket);
                return null;

            }
            string hashed = messageArgArr[1];

            if (hash.Equals(hashed)) {
                if (!UserSocketDictionary.ContainsKey(socket))
                    return new User(username, 1500);
                Console.WriteLine("Already Logged In!");
            }
            return null;
        }
        private static void GetDeckNames(Socket sock) {
            Console.WriteLine("Entered Deck Name Retrieval");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
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
        }
        private static void GetDeckContent(Socket sock, string deckname) {
            Console.WriteLine("Entered Deck Content Retrieval");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT d.Deck FROM Deck d, User u WHERE @us = u.Username AND u.Username = d.Username";
                command.Parameters.AddWithValue("@us", UserSocketDictionary[sock].Username);
                String cards = "";
                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        cards = reader.GetString(0);
                sock.Send(Encoding.ASCII.GetBytes("deckcontent\r\n" + cards));
                connection.Close();
            }
        }
        private static void SaveDeckContent(Socket sock, string deckname, string deck) {
            Console.WriteLine("Entered Deck Saving...");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Deck (Username, Deckname, Deck) VALUES (@u, @dn, @d) ON DUPLICATE KEY UPDATE Deck=@d";
                command.Parameters.AddWithValue("@u", UserSocketDictionary[sock].Username);
                command.Parameters.AddWithValue("@dn", deckname);
                command.Parameters.AddWithValue("@d", deck);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private static void HandleDisconnect(Socket handler) {
            Connections.Remove(handler);
            MatchmakingSockets.Remove(handler);
            if (UserSocketDictionary.ContainsKey(handler))
                Users.Remove(UserSocketDictionary[handler]);
            UserSocketDictionary.Remove(handler);
            PrintConnections();
        }
    }
    class User {
        public readonly string Username;
        public readonly int Skill;
        public Match? Match = null;
        public User(string u, int sk) { Username = u; Skill = sk; }
    }
    class Match {
        public readonly Socket Host;
        public Socket Client;
        public string Hostoutcome = "";
        public string Clientoutcome = "";
        public Match(Socket h, Socket c) { Host = h; Client = c; }
    }
}