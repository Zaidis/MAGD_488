using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;

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
    private List<Card> cards;

    public GameObject CreatureTokenPrefab;
    public GameObject SpellTokenPrefab;
    public GameObject ArtifactTokenPrefab;

    public Tile[] hostBoard = new Tile[2*5];
    public Tile[] clientBoard = new Tile[2*5];

    public int maxMana;
    public int currentMana = 10;

    public bool needsToSelectTile;
    public Card selectedCard; //for placement and making a token

    private void Awake() {
        if(_singleton == null) {
            _singleton = this;
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        _networkManager = NetworkManager.Singleton;
        if (!_networkManager.IsClient && !_networkManager.IsServer && !_networkManager.IsHost)
            _networkManager.StartHost();

        cards = new List<Card>(Resources.LoadAll("", typeof(Card)).Cast<Card>().ToArray());

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
        //myHand.myCards.Add(deck[rand]);
        myHand.AddCardToHand(deck[rand]);
        deck.RemoveAt(rand);
    }

    public GameObject NewToken(int cardID) //Creates Token based on card type and return gameObject
    {        
        Card card = cards.Find(c => c.ID == cardID);
        GameObject tokenObject = null;
        if (card is Creature creature) {
            tokenObject = Instantiate(CreatureTokenPrefab);
            CreatureToken creatureToken = tokenObject.GetComponent<CreatureToken>();
            creatureToken.creature = creature;
            creatureToken.ApplyCard();
        } else if (card is Spell spell) {
            tokenObject = Instantiate(SpellTokenPrefab);
            SpellToken spellToken = tokenObject.GetComponent<SpellToken>();
            spellToken.spell = spell;
            spellToken.ApplyCard();
        } else if (card is Artifact artifact) {
            tokenObject = Instantiate(ArtifactTokenPrefab);
            ArtifactToken artifactToken = tokenObject.GetComponent<ArtifactToken>();
            artifactToken.artifact = artifact;
            artifactToken.ApplyCard();
        }
        return tokenObject;
    }
    public void PlaceCard(bool isHost, int cardID, int x, int y) {
        GameObject token = NewToken(cardID);        
        if (isHost) {
            hostBoard[x + y * 5].SetToken(token);         
        } else {
            clientBoard[x + y * 5].SetToken(token);
        }
    }
    public void Attack(int x1, int y1, int x2, int y2) {
        if (_networkManager.IsHost) {
            CreatureToken t_one = hostBoard[x1 + y1 * 5].token.GetComponent<CreatureToken>();
            t_one.creature.OnAttack(hostBoard, clientBoard, new Vector2Int(x1, y1), new Vector2Int(x2, y2), true);
            CreatureToken t_two = clientBoard[x2 + y2 * 5].token.GetComponent<CreatureToken>();

            t_one.currentHealth -= t_two.currentAttack;
            t_two.currentHealth -= t_one.currentAttack;

            if (t_one.currentHealth <= 0) Destroy(hostBoard[x1 + y1 * 5].token);
            if (t_two.currentHealth <= 0) Destroy(clientBoard[x2 + y2 * 5].token);
        }
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

    public void TestCardPlace(int x) {
        System.Random rand = new System.Random();
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.PlaceCard(x, rand.Next(5), rand.Next(2));
    }

    public void TestDrawCard(Card card) {
        myHand.AddCardToHand(card);
    }

}
