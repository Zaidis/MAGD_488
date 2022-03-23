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
        private static string KLocalIp = "10.0.3.201"; //Local IP
        private const int KPort = 2552; //Port selected

        private static readonly List<User> Users = new List<User>();
        private static readonly List<Match> Matches = new List<Match>();
        private static List<User> MatchmakingUsers = new List<User>();

        private static RSACryptoServiceProvider csp = new RSACryptoServiceProvider(2048);
        private static RSAParameters privKey;
        private static RSAParameters pubKey;
        private static string pubKeyString = "";

        private static Object SQLLock = new Object();
        private static Object MatchmakingLock = new Object();
        static void Main() {
            IPAddress? ip = Dns.GetHostEntry(Dns.GetHostName()).AddressList[0]; //Set KLocalIp to detected localip
            if (ip.AddressFamily == AddressFamily.InterNetwork)
                KLocalIp = ip.ToString();

            IPAddress ipAddress = IPAddress.Parse(KLocalIp); //create ip, EP, and listener socket
            IPEndPoint localEp = new IPEndPoint(ipAddress, KPort);
            Socket listener = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

            privKey = csp.ExportParameters(true); //generate RSA Keypair, and generate xml including public key
            pubKey = csp.ExportParameters(false);
            StringWriter sw = new System.IO.StringWriter();
            XmlSerializer xs = new System.Xml.Serialization.XmlSerializer(typeof(RSAParameters));
            xs.Serialize(sw, pubKey);
            pubKeyString = sw.ToString();                       

            listener.Bind(localEp); //Bind to local ip and port, listen, and then handle new connections
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
            byte[] buffer = new byte[1024];
            bool loggedIn = false;
            handler.Send(Encoding.ASCII.GetBytes("rsakey\r\n" + pubKeyString));
            User? user = null;
            for (; ; ) {
                if (!Users.Any(u => u.socket == handler) && user != null) //Exits loop if user exists but is not in Users, or starts exit if handler is not connected
                    break;
                if (!handler.Connected) {
                    Console.WriteLine("Unexpected Disconnect Occured!");
                    HandleDisconnect(user);
                    break;
                }

                Thread.Sleep(1);
                PrintConnections();
                int numBytesReceived;
                try {
                    numBytesReceived = handler.Receive(buffer);
                } catch (SocketException e) {
                    Console.WriteLine(e);
                    HandleDisconnect(user);
                    break;
                }
                string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);

                if (messageArgArr[0].Equals("matchmake", StringComparison.OrdinalIgnoreCase)) { //Adding connection to matchmaking queue
                    if (loggedIn) {
                        if (!MatchmakingUsers.Contains(user!)) {
                            MatchmakingUsers.Add(user!); //add struct which contains needed info to matchmaking list
                            Matchmaking(user!);
                        }
                    }
                } else if (messageArgArr[0].Equals("login", StringComparison.OrdinalIgnoreCase)) {
                    if (!loggedIn) {
                        user = Login(messageArgArr[1], handler);
                        if (user != null) {
                            handler.Send(Encoding.ASCII.GetBytes("logingood\r\n"));
                            Users.Add(user);
                            loggedIn = true;
                        } else
                            handler.Send(Encoding.ASCII.GetBytes("loginbad\r\n"));
                    }
                } else if (messageArgArr[0].Equals("newaccount", StringComparison.OrdinalIgnoreCase)) {
                    handler.Send(NewUser(messageArgArr[1], messageArgArr[2], messageArgArr[3]) ? Encoding.ASCII.GetBytes("creationgood\r\n") : Encoding.ASCII.GetBytes("creationbad\r\n"));

                } else if (messageArgArr[0].Equals("outcome", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        MatchOutcome(messageArgArr[1], user!);
                } else if (messageArgArr[0].Equals("getdecknames", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        GetDeckNames(user!);
                } else if (messageArgArr[0].Equals("getdeckcontent", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        GetDeckContent(user!, messageArgArr[1]);
                } else if (messageArgArr[0].Equals("savedeck", StringComparison.OrdinalIgnoreCase)) {
                    if (loggedIn)
                        SaveDeckContent(user!, messageArgArr[1], messageArgArr[2]);
                } else if (messageArgArr[0].Equals("quit", StringComparison.OrdinalIgnoreCase)) { //Exit case
                    HandleDisconnect(user!);
                }
            }
            handler.Shutdown(SocketShutdown.Both);
            handler.Close();
        }
        private static void PrintConnections() //Print current connection statuses
        {
            Console.Clear();
            if (Users.Count > 0) {
                Console.WriteLine("Logged in Clients: ");
                foreach (User user in Users)
                    Console.WriteLine(user.socket.RemoteEndPoint + " : " + user.Username + " : Skill : " + user.Skill);
            }
            if (MatchmakingUsers.Count > 0) {
                Console.WriteLine("Matchmaking Clients: ");
                foreach (User user in MatchmakingUsers)
                    Console.WriteLine(user.socket.RemoteEndPoint + " : " + user.Username + " : Skill : " + user.Skill);
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
        private static void Matchmaking(User user) //Function responsible for matchmaking loop, runs continuously, if at least 2 users are in the queue, match closest two in skill, add to a match, and remove from pool
        {
            lock (MatchmakingLock) {
                if (MatchmakingUsers.Count > 1 && user.Match == null) { //matchmake if users matchmaking > 1
                    Console.WriteLine("Attempting to Match");
                    byte[] buffer = new byte[1024];
                    User host = MatchmakingUsers[1];
                    User client = MatchmakingUsers[0];

                    int minDifference = int.MaxValue; //find two users with closest skill, match together
                    MatchmakingUsers = MatchmakingUsers.OrderBy(u => u.Skill).ToList();
                    for (int i = 1; i < MatchmakingUsers.Count; i++) { 
                        int currentDifference = Math.Abs(MatchmakingUsers[i].Skill - MatchmakingUsers[i].Skill);
                        if (currentDifference < minDifference) {
                            minDifference = currentDifference;
                            host = MatchmakingUsers[i];
                            client = MatchmakingUsers[i - 1];
                        }
                    }

                    host.socket.Send(Encoding.ASCII.GetBytes("start\r\n" + client.Username)); //Send start command to selected host
                    Console.WriteLine("Start command sent to host");
                    int numBytesReceived = 0;
                    try {
                        numBytesReceived = user.socket.Receive(buffer);
                    } catch (SocketException e) {
                        Console.WriteLine(e);
                        HandleDisconnect(user);
                        return;
                    }
                    string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
                    string[] messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
                    Console.WriteLine(textReceived);
                    if (messageArgArr[0].Equals("code")) {
                        Console.WriteLine("Sent " + "connect\r\n" + messageArgArr[1] + "\nto " + client.socket.RemoteEndPoint + " : " + client.Username + " : Skill : " + client.Skill);
                        client.socket.Send(Encoding.ASCII.GetBytes("connect\r\n" + messageArgArr[1] + "\r\n" + host.Username));
                        Console.WriteLine("Sent connection message to client");

                        Match newMatch = new Match(host, client);
                        Matches.Add(newMatch);
                        host.Match = newMatch;
                        client.Match = newMatch;

                        MatchmakingUsers.Remove(host);
                        MatchmakingUsers.Remove(client);
                    } else if (messageArgArr[0].Equals("quit", StringComparison.OrdinalIgnoreCase)) //Exit case
                        HandleDisconnect(host);
                }
            }
        }
        private static void MatchOutcome(string outcome, User comm) //set outcome of match of passed User according to if socket is host or client in match, then eval
        {
            if (comm.Match != null) {
                Match match = comm.Match!;
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
            if (username == "" || Users.Any(u => u.Username == username))
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
                return null;
            }
            string textReceived = Encoding.ASCII.GetString(buffer, 0, numBytesReceived); //decode from stream to ASCII
            string[] messageArgArr;
            try { //try catch to handle unexepected text that isn't cyphertext
                var bytesCypherText = Convert.FromBase64String(textReceived);
                csp.ImportParameters(privKey);
                var bytesPlainTextData = csp.Decrypt(bytesCypherText, false);
                textReceived = System.Text.Encoding.Unicode.GetString(bytesPlainTextData);
                messageArgArr = textReceived.Split(StringSeparators, StringSplitOptions.None);
            } catch (Exception e) {
                Console.WriteLine(e);
                return null;

            }
            string hashed = messageArgArr[1];

            if (hash.Equals(hashed)) {
                if (!Users.Any(u => u.socket == socket))
                    return new User(username, 1500, socket);
                Console.WriteLine("Already Logged In!");
            }
            return null;
        }
        private static void GetDeckNames(User user) {
            Console.WriteLine("Entered Deck Name Retrieval");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT d.Deckname FROM Deck d, User u WHERE @us = u.Username AND u.Username = d.Username";
                command.Parameters.AddWithValue("@us", user.Username);
                String message = "decknames\r\n";
                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        message = message + reader.GetString(0) + "\r\n";
                message = message.Substring(0, message.LastIndexOf("\r\n"));
                user.socket.Send(Encoding.ASCII.GetBytes(message));
                connection.Close();
            }
        }
        private static void GetDeckContent(User user, string deckname) {
            Console.WriteLine("Entered Deck Content Retrieval");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"SELECT d.Deck FROM Deck d, User u WHERE @us = u.Username AND u.Username = d.Username";
                command.Parameters.AddWithValue("@us", user.Username);
                String cards = "";
                using (SqliteDataReader reader = command.ExecuteReader())
                    while (reader.Read())
                        cards = reader.GetString(0);
                user.socket.Send(Encoding.ASCII.GetBytes("deckcontent\r\n" + cards));
                connection.Close();
            }
        }
        private static void SaveDeckContent(User user, string deckname, string deck) {
            Console.WriteLine("Entered Deck Saving...");
            using SqliteConnection connection = new SqliteConnection("Data Source=Mythos.db");
            lock (SQLLock) {
                connection.Open();
                SqliteCommand command = connection.CreateCommand();
                command.CommandText = @"INSERT INTO Deck (Username, Deckname, Deck) VALUES (@u, @dn, @d) ON DUPLICATE KEY UPDATE Deck=@d";
                command.Parameters.AddWithValue("@u", user.Username);
                command.Parameters.AddWithValue("@dn", deckname);
                command.Parameters.AddWithValue("@d", deck);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }
        private static void HandleDisconnect(User? user) {
            if (user != null) {
                MatchmakingUsers.Remove(user);
                Users.Remove(user);
            }
            PrintConnections();
        }
    }
    class User {
        public Socket socket;
        public readonly string Username;
        public readonly int Skill;
        public Match? Match = null;
        public User(string u, int sk, Socket s) { Username = u; Skill = sk; socket = s; }
    }
    class Match {
        public User Host;
        public User Client;
        public string Hostoutcome = "";
        public string Clientoutcome = "";
        public Match(User h, User c) { Host = h; Client = c; }
    }
}