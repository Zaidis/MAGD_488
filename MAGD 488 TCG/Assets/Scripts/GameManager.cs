using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
public class GameManager : MonoBehaviour
{
    private static System.Random rng = new System.Random();

    private static GameManager _singleton;
    public static GameManager Singleton { get { return _singleton; } }
    public TextMeshProUGUI TurnStatus;
    public TextMeshProUGUI opponent;
    public GameObject Connecting;
    public Button NextTurn;
    private NetworkManager _networkManager;
    public bool IsHostTurn = true;
    public string yourName;
    private string opponentName; //TODO IMPLEMENT

    public Hand myHand;
    public List<Card> deck;

    [SerializeField] private GameObject TokenPrefab;

    public Tile[][] hostBoard;
    public Tile[][] clientBoard;
    private void Start() {
        _networkManager = NetworkManager.Singleton;
        if (!_networkManager.IsClient && !_networkManager.IsServer && !_networkManager.IsHost)
            _networkManager.StartHost();
        _singleton = this;        
        TurnStatus = GameObject.Find("TurnStatus").GetComponent<TextMeshProUGUI>();
        NextTurn = GameObject.Find("NextTurn").GetComponent<Button>();
        if (_networkManager.IsHost) {
            TurnStatus.text = "Your Turn!";
            NextTurn.interactable = true;
            
        } else {
            TurnStatus.text = "Other Player's Turn.";
        }
        opponentName = MythosClient.instance.opponentUserName;
        opponent.text = "Opponent: " + opponentName;
        StartCoroutine(ClearConnectingOnConnect());        
    }

    
    private void BeginGame() {
        /*
         * The player who goes first will draw 3 cards. 
         * The player who goes second will draw 4 cards. 
         */
        ShuffleDeck(deck);
        int drawCardAmount = _networkManager.IsHost ? 3 : 4; //adam did this

        for (int i = 0; i < drawCardAmount; i++) {
            DrawTopCard(deck);
        }
        //mulligan

    }

    /// <summary>
    /// Shuffles the deck. 
    /// </summary>
    /// <param name="deck"></param>
    public void ShuffleDeck(List<Card> deck) {         
        int n = deck.Count;
        while (n > 1) {
            n--;
            int k = rng.Next(n + 1);
            var value = deck[k];
            deck[k] = deck[n];
            deck[n] = value;
        }
    }

    public void DrawTopCard(List<Card> deck) {
        myHand.myCards.Add(deck[0]);
        deck.RemoveAt(0);
    }

    public void DrawRandomCard(List<Card> deck) {
        int rand = rng.Next(deck.Count);
        myHand.myCards.Add(deck[rand]);
        deck.RemoveAt(rand);
    }

    public Token NewToken(int cardID) {
        Token token = new Token();
        Card card = deck.Find(c => c.ID == cardID);
        if(card.type == cardType.creature) {
            Creature c = (Creature)card;
            token.currentHealth = c.defaultHealthAmount;
            token.currentAttack = c.defaultPowerAmount;
        } else if(card.type == cardType.artifact) {
            Artifact a = (Artifact)card;
            token.currentHealth = a.defaultHealthAmount;
        }

        return token;
    }
    public void PlaceCard(bool isHost, int cardID, int x, int y) {
        Token token = NewToken(cardID);        
        if (isHost) {
            hostBoard[x][y].token = token;            
        } else {
            clientBoard[x][y].token = token;
        }
    }
    public void Attack(int x1, int y1, int x2, int y2) {

        if (_networkManager.IsHost) {

            Token t_one = hostBoard[x1][y1].token;
            Token t_two = clientBoard[x2][y2].token;

            t_one.currentHealth -= t_two.currentAttack;
            t_two.currentHealth -= t_one.currentAttack;

            if (t_one.currentHealth <= 0) DeleteToken(hostBoard[x1][y1]);
            if (t_two.currentHealth <= 0) DeleteToken(clientBoard[x2][y2]);

        }

    }

    /// <summary>
    /// Called when a token has died / ran out of health.
    /// </summary>
    public void DeleteToken(Tile tile) {
        Destroy(tile.token.gameObject);
        tile.token = null;
    }

    public void OnNextTurnPressed() {
        TurnStatus.text = "Other User's Turn";
        IsHostTurn = IsHostTurn ? false : true;
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.NextTurnPressed(IsHostTurn);
    }    
    private IEnumerator ClearConnectingOnConnect() { //Guarentees that connecting screen is shown until all users are connected and it's safe to start gameplay
        if (_networkManager.IsServer) {
            while (_networkManager.ConnectedClients.Count < 2)
                yield return null;
        } else {
            while(!_networkManager.IsClient)
                yield return null;
        }
        Connecting.SetActive(false);
        BeginGame();
    }


    [System.Serializable]
    public class Hand {

        public List<Card> myCards = new List<Card>();

    }
}
