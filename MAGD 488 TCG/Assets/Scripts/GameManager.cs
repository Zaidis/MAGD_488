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
    public NetworkManager _networkManager;
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

   

    public bool needsToSelectTile;
    public bool isAttecking;
    public CreatureToken selectedCreature;

    public Card selectedCard; //for placement and making a token
    public int selectedCardNumber; //to know which card in your hand will be removed

    public bool isHost;

    public Card_Popup popup; //when you right click a card

    #region Mana
        public int maxMana;
        public int currentMana;
        [SerializeField] private TextMeshProUGUI manaText;
    #endregion

    #region Creature Options
    //When clicking on a creature, these buttons will appear. 
    public O_AttackToken attackTokenOption;
        public O_AttackPlayer attackPlayerOption;
        public O_Ability abilityOption;
        [SerializeField] private Transform[] optionSpawnLocations;
        [SerializeField] private GameObject optionsParent;
    #endregion

    #region Player Variables
        public int hostHealth = 20;
        public int clientHealth = 20;
        [SerializeField] TextMeshPro hostHealthText;
        [SerializeField] TextMeshPro clientHealthText;
    #endregion

    #region Tile Materials
    public Material m_default;
    public Material m_active;
    public Material m_deactive;
    #endregion
    private void Awake() {
        if(_singleton == null) {
            _singleton = this;
        } else {
            Destroy(this);
        }
    }

    private void Start() {
        
        AffectCurrentMana(20);
        
        //isHost = true;
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

        isHost = NetworkManager.Singleton.IsHost;
        if (!isHost) {
            //flip text 180
            hostHealthText.transform.rotation = Quaternion.Euler(90, 180, 0);
            clientHealthText.transform.rotation = Quaternion.Euler(90, 180, 0);
        }

    }

    /// <summary>
    /// Add or remove mana. 
    /// </summary>
    public void AffectCurrentMana(int amount) {
        
        currentMana += amount;
        manaText.text = "Current Mana: " + currentMana.ToString();

    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="hostAmount">The amount of health that will affect the host.</param>
    /// <param name="clientAmount">The amount of health that will affect the client.</param>
    public void AffectHealthValues(int hostAmount, int clientAmount) {
        hostHealth += hostAmount;
        clientHealth += clientAmount;

        hostHealthText.text = hostHealth.ToString();
        clientHealthText.text = clientHealth.ToString();
    }

    public void ResetSelectedCard() {
        needsToSelectTile = false;
        selectedCard = null;
        selectedCardNumber = -5;
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
        //myHand.myCards.Add(deck[0]);
        if(deck[0] != null) {
            myHand.AddCardToHand(deck[0]);
            deck.RemoveAt(0);
        }
        
    }

    public void DrawTopCard() {
        //myHand.myCards.Add(deck[0]);
        if (deck[0] != null) {
            myHand.AddCardToHand(deck[0]);
            deck.RemoveAt(0);
        }

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
        } else if (card is Artifact artifact) {
            tokenObject = Instantiate(ArtifactTokenPrefab);
            ArtifactToken artifactToken = tokenObject.GetComponent<ArtifactToken>();
            artifactToken.artifact = artifact;
            artifactToken.ApplyCard();
        }

        return tokenObject;
    }



    public void PlaceCard(bool isHost, int cardID, int id) {
        GameObject token = NewToken(cardID);        
        if (isHost) {
            if(hostBoard[id].token == null)
                hostBoard[id].SetToken(token);         
        } else {
            if(clientBoard[id].token == null)
                clientBoard[id].SetToken(token);
        }
    }



    public void Attack(int attackerID, int attackedID, bool attackingFromHostSide) {
        if (attackingFromHostSide) {
            CreatureToken t_one = hostBoard[attackerID].token.GetComponent<CreatureToken>();
            t_one.AttackWithToken(clientBoard[attackedID]);
        } else {
            CreatureToken t_one = clientBoard[attackerID].token.GetComponent<CreatureToken>();
            t_one.AttackWithToken(hostBoard[attackedID]);
        }
    }

    public void OnNextTurnPressed() {
        TurnStatus.text = "Other User's Turn";
        IsHostTurn = IsHostTurn ? false : true;
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.UpdateTurnServerRpc(IsHostTurn);
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

    /*public void TestCardPlace(int x) {
        System.Random rand = new System.Random();
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.PlaceCard(x, rand.Next(5), rand.Next(2));
    } */

    public void TestDrawCard(Card card) {
        myHand.AddCardToHand(card);
    }

    /// <summary>
    /// Called When attacking a creature. Sets opponents tiles to active if attackable. 
    /// </summary>
    /// <param name="board">Specific board to turn active.</param>
    /// <param name="isMelee">Whether or not the attacking creature is melee.</param>
    /// <param name="attackerID">The creature that is attacking.</param>
    public void ChangeTilesMaterial(Tile[] board, bool isMelee, int attackerID) {
        if (isMelee) {
             for(int i = 9; i > 4; i--) {
                Tile t = board[i].GetComponent<Tile>();
                Tile t2 = board[i - 5].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeMaterial(m_active);
                    t.active = true;
                } else if(t2.token != null) { //checks the tile behind
                    t2.ChangeMaterial(m_active);
                    t2.active = true;
                }
            }
        } else { //if you are ranged, check ALL tiles
            for(int i = 0; i < board.Length; i++) {
                Tile t = board[i].GetComponent<Tile>();
                if(t.token != null) {
                    t.ChangeMaterial(m_active);
                    t.active = true;
                }
            }
        }
    }


    public bool CheckIfCreatureCanAttackPlayer(Tile[] board, int attackerID) {
        //checks to see if you can attack the opponent with this particular creature. 
        //in order to attack the opponent, the entire enemy column that matches the attacking creature must be empty.
        //that goes for both melee and ranged creatures. The difference is ranged can attack the backline even if
        //there is a melee creature in front.
        
        if(attackerID >= 0 && attackerID <= 4) {
            //this creature is in the back row
            if(board[attackerID].GetComponent<Tile>().token == null && board[attackerID + 5].GetComponent<Tile>().token == null) {
                return true;
            }
        } else if(attackerID >= 5 && attackerID <= 9) {
            if (board[attackerID].GetComponent<Tile>().token == null && board[attackerID - 5].GetComponent<Tile>().token == null) {
                
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// All tiles become unactive.
    /// </summary>
    public void ResetAllTiles(Tile[] board) {
        for(int i = 0; i < board.Length; i++) {
            Tile t = board[i].GetComponent<Tile>();
            t.ChangeMaterial(m_default);
            t.active = false;
        }
    }

    public bool CheckIfMyCreature(Tile[] board, Tile t) {
        for(int i = 0; i < board.Length; i++) {
            if (t == board[i]) return true;
        }

        return false;
    }

    /// <summary>
    /// When I click on a creature, these buttons will apear for what is available.
    /// </summary>
    public void CreatureOptionButtons(CreatureToken token, bool isHost) {
        int counter = 0;
        attackTokenOption.token = token;
        attackPlayerOption.token = token;

        attackTokenOption.gameObject.SetActive(false);
        attackPlayerOption.gameObject.SetActive(false);
        abilityOption.gameObject.SetActive(false);

        if (isHost) {
            if (token.hasAttacked == false) {
                //add attack button
                attackTokenOption.transform.position = optionSpawnLocations[counter].position;
                attackTokenOption.gameObject.SetActive(true);
                counter++;

                if (CheckIfCreatureCanAttackPlayer(clientBoard, token.transform.parent.GetComponent<Tile>().GetTileID())) {
                    //add attack player button

                    attackPlayerOption.transform.position = optionSpawnLocations[counter].position;
                    attackPlayerOption.gameObject.SetActive(true);
                    counter++;
                }
            }

            //ability
        }
        

        
    }

    
}
