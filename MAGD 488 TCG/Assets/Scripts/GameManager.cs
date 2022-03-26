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
    void Start() {
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

        //draw bottom card == list[0]

        myHand.myCards.Add(deck[0]);
        deck.RemoveAt(0);
    }

    public void DrawRandomCard(List<Card> deck) {
        //draw random card
        int rand = rng.Next(deck.Count);

        myHand.myCards.Add(deck[rand]);
        deck.RemoveAt(rand);
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
