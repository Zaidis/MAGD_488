using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
public class GameManager : MonoBehaviour {
    private static System.Random rng = new System.Random();

    private static GameManager _singleton;
    public static GameManager Singleton { get { return _singleton; } }
    public TextMeshProUGUI TurnStatus;
    public Player player;

    public PlayableDirector turnAnim;
    public GameObject turnAnimObject;


    public TextMeshProUGUI opponent;
    public TextMeshProUGUI username;
    public GameObject Connecting;
    public Button NextTurn;
    public NetworkManager _networkManager;
    public bool IsHostTurn = true;
    public string yourName;
    private string opponentName; //TODO IMPLEMENT

    public Hand myHand;
    public OpponentHand opponentHand;
    public List<Card> deck = new List<Card>();
    private List<Card> cards;
    private Dictionary<int, Card> dictionaryOfCards = new Dictionary<int, Card>();


    public GameObject[] CreatureTokenPrefab;
    public GameObject SpellTokenPrefab;
    public GameObject[] ArtifactTokenPrefab;

    public Tile[] hostBoard = new Tile[2 * 5];
    public Tile[] clientBoard = new Tile[2 * 5];

    public string winScene;
    public string loseScene;

    public bool needsToSelectTile;
    public bool isAttecking;
    public bool isUsingAbility;
    public CreatureToken selectedCreature;

    public Card selectedCard; //for placement and making a token
    public int selectedCardNumber; //to know which card in your hand will be removed

    public bool isHost;

    public Card_Popup panelPopup; //when you right click a card
    public Hover_Popup cardPopup; //when you hover over a token

    [Header("Compass Variables")]
    public Compass compass;
    public int compassYRotation;

    [Header("Attribute Tool Tips")]
    public Tooltip tooltip;

    #region End Turn Buttons
    [Header("End Turn Buttons")]
    public GameObject hostButton;
    public GameObject clientButton;
    public EndTurnButton myEndTurnButton;
    #endregion


    #region Mana
    [Header("Mana Values")]
    public GameObject hostManaParent;
    public GameObject clientManaParent;

    public int hostMaxMana;
    public int hostCurrentMana;
    public int clientMaxMana;
    public int clientCurrentMana;
    [SerializeField] private TextMeshPro hostCurrentManaText;
    [SerializeField] private TextMeshPro clientCurrentManaText;

    [SerializeField] private TextMeshPro hostMaxManaText;
    [SerializeField] private TextMeshPro clientMaxManaText;
    #endregion

    #region Creature Options

    [Header("Creature Values")]

    //When clicking on a creature, these buttons will appear. 
    public Shader defaultShader; //for tokens

    public O_AttackToken attackTokenOption;
    public O_AttackPlayer attackPlayerOption;
    public O_Ability abilityOption;
    [SerializeField] private Transform[] hostOptionSpawnLocations;
    [SerializeField] private Transform[] clientOptionSpawnLocations;
    [SerializeField] private GameObject optionsParent;





    #endregion

    #region Player Variables
    public int hostHealth;
    public int clientHealth;
    public int maxHostHealth = 20;
    public int maxClientHealth = 20;
    [SerializeField] TextMeshPro hostHealthText;
    [SerializeField] TextMeshPro clientHealthText;

    public Transform hostHealthGemPosition;
    public Transform clientHealthGemPosition;
    #endregion

    #region Tile Materials
    public Material m_default;
    public Material m_active;
    public Material m_deactive;
    public Material m_selected;
    #endregion
    private void Awake() {
        if (_singleton == null) {
            _singleton = this;
        } else {
            Destroy(this);
        }
    }
    private void Start() {

        AffectManaValues(1, 0, 1, 0); //INITIAL MANA

        //isHost = true;
        _networkManager = NetworkManager.Singleton;
        if (!_networkManager.IsClient && !_networkManager.IsServer && !_networkManager.IsHost)
            _networkManager.StartHost();

        cards = new List<Card>(Resources.LoadAll("", typeof(Card)).Cast<Card>().ToArray());
        foreach (Card card in cards) {
            dictionaryOfCards.Add(card.ID, card);
        }
        //TurnStatus = GameObject.Find("TurnStatus").GetComponent<TextMeshProUGUI>();
        // NextTurn = GameObject.Find("NextTurn").GetComponent<Button>();
        if (_networkManager.IsHost) {
            TurnStatus.text = "Your Turn!";
            NextTurn.interactable = true;
            YourTurnAnimation();
        } else {
            //TurnStatus.text = "Other Player's Turn.";
        }
        if (!MythosClient.instance.opponentUserName.Equals("")) {
            opponentName = MythosClient.instance.opponentUserName;

            opponent.text = opponentName;
            username.text = MythosClient.instance.userName;
        } else {
            opponent.text = "Join Code: " + MythosClient.instance.joinCode;
        }

        StartCoroutine(ClearConnectingOnConnect());

        isHost = NetworkManager.Singleton.IsHost;
        if (!isHost) {
            //flip text 180
            hostHealthText.transform.rotation = Quaternion.Euler(90, 180, 0);
            clientHealthText.transform.rotation = Quaternion.Euler(90, 180, 0);
            hostManaParent.transform.rotation = Quaternion.Euler(0, 180, 0);
            clientManaParent.transform.rotation = Quaternion.Euler(0, 180, 0);

            attackPlayerOption.transform.rotation = Quaternion.Euler(0, 180, 0);
            attackTokenOption.transform.rotation = Quaternion.Euler(0, 180, 0);
            abilityOption.transform.rotation = Quaternion.Euler(0, 180, 0);

        }

        //affect deck
        if (TempDeck.instance.deckID.Count > 0) {
            deck.Clear();
            for (int i = 0; i < TempDeck.instance.deckID.Count; i++)
                deck.Add(dictionaryOfCards[TempDeck.instance.deckID[i]]);
        }

        AffectHealthValues(20, 20);
    }

    public void YourTurnAnimation() {
        turnAnimObject.SetActive(true);
        turnAnim.Play();
        Invoke("TurnOffAnimation", 5f);
    }

    public void TurnOffAnimation() {
        turnAnimObject.SetActive(false);
    }


    /// <summary>
    /// Add or remove mana. 
    /// </summary>
    public void AffectManaValues(int hostAmount, int clientAmount, int hostMax, int clientMax) {

        //currentMana += amount;
        //manaText.text = "Current Mana: " + currentMana.ToString();

        hostMaxMana = hostMax;
        hostCurrentMana = hostAmount;

        clientMaxMana = clientMax;
        clientCurrentMana = clientAmount;

        hostMaxManaText.text = hostMaxMana.ToString();
        clientMaxManaText.text = clientMaxMana.ToString();

        hostCurrentManaText.text = hostCurrentMana.ToString();
        clientCurrentManaText.text = clientCurrentMana.ToString();



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

        if (hostHealth <= 0) {    //Check for eitherplayer death, load respective scene on either with a safe, delayed, scene change
            MythosClient.instance.OnOutcome(false);
            if (isHost)
                StartCoroutine(SafeSceneChange(1, loseScene));
            else
                StartCoroutine(SafeSceneChange(1, winScene));
        } else if (clientHealth <= 0) {
            MythosClient.instance.OnOutcome(true);
            if (!isHost)
                StartCoroutine(SafeSceneChange(1, loseScene));
            else
                StartCoroutine(SafeSceneChange(1, winScene));
        }
    }

    public void ResetSelectedCard() {
        needsToSelectTile = false;
        selectedCard = null;
        selectedCardNumber = -5;
    }

    private void BeginGame() {
        player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        /*
         * The player who goes first will draw 3 cards. 
         * The player who goes second will draw 4 cards. 
         */
        ShuffleDeck(deck);
        int drawCardAmount = _networkManager.IsHost ? 3 : 4; //adam did this

        for (int i = 0; i < drawCardAmount; i++) {
            Debug.Log("Drawing Initial Card");
            DrawTopCard(deck);
        }

        myHand.PositionCards();
        //mulligan

        if (isHost) {
            hostButton.SetActive(true);
            myEndTurnButton = hostButton.transform.GetChild(0).GetComponent<EndTurnButton>();
        } else {
            clientButton.SetActive(true);
            myEndTurnButton = clientButton.transform.GetChild(0).GetComponent<EndTurnButton>();
        }
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
        if (deck.Count >= 1) {
            if (deck[0] != null) {
                myHand.AddCardToHand(deck[0]);
                deck.RemoveAt(0);
            }
        }


    }

    public void DrawTopCard() {
        //myHand.myCards.Add(deck[0]);
        if (deck.Count >= 1) {
            if (deck[0] != null) {
                myHand.AddCardToHand(deck[0]);
                deck.RemoveAt(0);
            }
        }


    }
    public GameObject NewToken(int cardID) //Creates Token based on card type and return gameObject
    {
        Card card = cards.Find(c => c.ID == cardID);
        GameObject tokenObject = null;
        if (card is Creature creature) {

            if (card.cardFaction == faction.empire) {
                tokenObject = Instantiate(CreatureTokenPrefab[0]);
            } else if (card.cardFaction == faction.beasts) {
                tokenObject = Instantiate(CreatureTokenPrefab[1]);
            } else if (card.cardFaction == faction.guidingLight) {
                tokenObject = Instantiate(CreatureTokenPrefab[2]);
            } else if (card.cardFaction == faction.hunted) {
                tokenObject = Instantiate(CreatureTokenPrefab[3]);
            } else if (card.cardFaction == faction.unaligned) {
                tokenObject = Instantiate(CreatureTokenPrefab[4]);
            }


            CreatureToken creatureToken = tokenObject.GetComponent<CreatureToken>();
            creatureToken.creature = creature;
            creatureToken.ApplyCard();
        } else if (card is Artifact artifact) {
            //tokenObject = Instantiate(ArtifactTokenPrefab);

            if (card.cardFaction == faction.empire) {
                tokenObject = Instantiate(ArtifactTokenPrefab[0]);
            } else if (card.cardFaction == faction.beasts) {
                tokenObject = Instantiate(ArtifactTokenPrefab[1]);
            } else if (card.cardFaction == faction.guidingLight) {
                tokenObject = Instantiate(ArtifactTokenPrefab[2]);
            } else if (card.cardFaction == faction.hunted) {
                tokenObject = Instantiate(ArtifactTokenPrefab[3]);
            } else if (card.cardFaction == faction.unaligned) {
                tokenObject = Instantiate(ArtifactTokenPrefab[4]);
            }

            ArtifactToken artifactToken = tokenObject.GetComponent<ArtifactToken>();
            artifactToken.artifact = artifact;
            artifactToken.ApplyCard();
        }



        return tokenObject;
    }



    public void PlaceCard(bool isHost, int cardID, int id) {
        GameObject token = NewToken(cardID);
        if (isHost) {
            if (hostBoard[id].token == null)
                hostBoard[id].SetToken(token);
        } else {
            if (clientBoard[id].token == null)
                clientBoard[id].SetToken(token);
        }
    }


    public void UseAbility(int userID, bool isHostSide) {
        if (isHostSide) {
            Token t = hostBoard[userID].token.GetComponent<Token>();

            if (t is CreatureToken c) {
                c.UseAbility();
                c.PlayParticles();
            } else if (t is ArtifactToken a) {
                a.UseAbility();
                a.PlayParticles();
            }
        } else {
            Token t = clientBoard[userID].token.GetComponent<Token>();

            if (t is CreatureToken c) {
                c.UseAbility();
                c.PlayParticles();
            } else if (t is ArtifactToken a) {
                a.UseAbility();
                a.PlayParticles();
            }
        }
    }

    public void UseTargetedAbility(int userID, int victimID, bool isHostSide) {


        if (isHostSide) {
            if (hostBoard[userID].token != null) {
                Token t = hostBoard[userID].token.GetComponent<Token>();

                if (t is CreatureToken c) {
                    if (c.creature.targetFriendly) {
                        c.UseTargetedAbility(hostBoard[victimID]);
                        c.PlayParticles();
                    } else if (c.creature.targetEnemy) {
                        c.UseTargetedAbility(clientBoard[victimID]);
                        c.PlayParticles();
                    } else {

                    }
                }
            }

        } else {
            if (clientBoard[userID].token != null) {
                Token t = clientBoard[userID].token.GetComponent<Token>();
                if (t is CreatureToken c) {

                    if (c.creature.targetFriendly) {
                        c.UseTargetedAbility(clientBoard[victimID]);
                        c.PlayParticles();
                    } else if (c.creature.targetEnemy) {
                        c.UseTargetedAbility(hostBoard[victimID]);
                        c.PlayParticles();
                    } else {

                    }
                    /*if (isHostSide) {
                        c.UseTargetedAbility(hostBoard[victimID]);
                        c.PlayParticles();
                    }
                    else {
                        c.UseTargetedAbility(clientBoard[victimID]);
                        c.PlayParticles();
                    }*/
                }
            }

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
            while (!_networkManager.IsConnectedClient)
                yield return null;
        }
        Connecting.SetActive(false);
        BeginGame();
    }

    public void ResetTokens(Tile[] board) {
        for (int i = 0; i < 10; i++) {
            if (board[i].token != null) {
                if (board[i].token.GetComponent<Token>() is CreatureToken c) {
                    c.ResetToken();
                } else if (board[i].token.GetComponent<Token>() is ArtifactToken a) {
                    a.ResetToken();
                }
            }
        }
    }





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
            for (int i = 9; i > 4; i--) {
                Tile t = board[i].GetComponent<Tile>();
                Tile t2 = board[i - 5].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeTokenMaterial(m_active);
                    t.active = true;
                } else if (t2.token != null) { //checks the tile behind
                    t2.ChangeTokenMaterial(m_active);
                    t2.active = true;
                }
            }
        } else { //if you are ranged, check ALL tiles
            for (int i = 0; i < board.Length; i++) {
                Tile t = board[i].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeTokenMaterial(m_active);
                    t.active = true;
                }
            }
        }
    }

    /// <summary>
    /// Activates all tiles with tokens on them on a specific board. 
    /// </summary>
    /// <param name="board"></param>
    public void ActivateTilesWithTokensInBoard(Tile[] board) {
        for (int i = 0; i < board.Length; i++) {
            Tile t = board[i].GetComponent<Tile>();
            if (t.token != null) {
                t.ChangeTokenMaterial(m_active);
                t.active = true;
            }
        }
    }

    /// <summary>
    /// Activates all tiles with tokens on both boards.
    /// </summary>
    public void ActivateAllTilesWithTokens() {
        for (int i = 0; i < hostBoard.Length; i++) {
            Tile t = hostBoard[i].GetComponent<Tile>();
            if (t.token != null) {
                t.ChangeTokenMaterial(m_active);
                t.active = true;
            }
        }
        for (int i = 0; i < clientBoard.Length; i++) {
            Tile t = clientBoard[i].GetComponent<Tile>();
            if (t.token != null) {
                t.ChangeTokenMaterial(m_active);
                t.active = true;
            }
        }
    }

    public void ActivateAllFriendlyTilesWithTokens() {
        if (isHost) {
            for (int i = 0; i < hostBoard.Length; i++) {
                Tile t = hostBoard[i].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeTokenMaterial(m_active);
                    t.active = true;
                }
            }
        } else {
            for (int i = 0; i < clientBoard.Length; i++) {
                Tile t = clientBoard[i].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeTokenMaterial(m_active);
                    t.active = true;
                }
            }
        }
    }

    public void ActivateAllEnemyTilesWithTokens() {
        if (isHost) {
            for (int i = 0; i < clientBoard.Length; i++) {
                Tile t = clientBoard[i].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeTokenMaterial(m_active);
                    t.active = true;
                }
            }
        } else {
            for (int i = 0; i < hostBoard.Length; i++) {
                Tile t = hostBoard[i].GetComponent<Tile>();
                if (t.token != null) {
                    t.ChangeTokenMaterial(m_active);
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

        if (attackerID >= 0 && attackerID <= 4) {
            //this creature is in the back row
            if (board[attackerID].GetComponent<Tile>().token == null && board[attackerID + 5].GetComponent<Tile>().token == null) {
                return true;
            }
        } else if (attackerID >= 5 && attackerID <= 9) {
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
        for (int i = 0; i < board.Length; i++) {
            Tile t = board[i].GetComponent<Tile>();
            t.ChangeTokenMaterial(m_default);
            t.active = false;
        }
    }

    public bool CheckIfMyCreature(Tile[] board, Tile t) {
        for (int i = 0; i < board.Length; i++) {
            if (t == board[i]) return true;
        }

        return false;
    }

    public void TurnOffOptionsAndUnselect() {
        attackTokenOption.gameObject.SetActive(false);
        attackPlayerOption.gameObject.SetActive(false);
        abilityOption.gameObject.SetActive(false);

        ResetAllTiles(hostBoard);
        ResetAllTiles(clientBoard);
    }
    /// <summary>
    /// When I click on a creature, these buttons will apear for what is available.
    /// </summary>
    public void CreatureOptionButtons(CreatureToken token, bool isHost) {
        if (isHost) {
            ResetAllTiles(hostBoard);
        } else {
            ResetAllTiles(clientBoard);
        }
        int counter = 0;
        attackTokenOption.token = token;
        attackPlayerOption.token = token;
        abilityOption.token = token;
        token.GetComponent<Token>().ChangeMaterial(m_selected);

        attackTokenOption.gameObject.SetActive(false);
        attackPlayerOption.gameObject.SetActive(false);
        abilityOption.gameObject.SetActive(false);

        if (isHost) {
            if (token.hasAttacked == false) {
                //add attack button
                attackTokenOption.transform.position = hostOptionSpawnLocations[counter].position;
                attackTokenOption.gameObject.SetActive(true);
                counter++;

                if (CheckIfCreatureCanAttackPlayer(clientBoard, token.transform.parent.GetComponent<Tile>().GetTileID())) {
                    //add attack player button

                    attackPlayerOption.transform.position = hostOptionSpawnLocations[counter].position;
                    attackPlayerOption.gameObject.SetActive(true);
                    counter++;
                }
            }

            if (token.creature.hasAbility) {
                if (token.creature.abilityCost <= hostCurrentMana) {
                    if (!token.castedAbility) {

                        abilityOption.transform.position = hostOptionSpawnLocations[counter].position;
                        abilityOption.gameObject.SetActive(true);
                        counter++;
                    }
                }

            }

            //ability
        } else {
            if (token.hasAttacked == false) {
                //add attack button
                attackTokenOption.transform.position = clientOptionSpawnLocations[counter].position;
                attackTokenOption.gameObject.SetActive(true);
                counter++;

                if (CheckIfCreatureCanAttackPlayer(hostBoard, token.transform.parent.GetComponent<Tile>().GetTileID())) {
                    //add attack player button

                    attackPlayerOption.transform.position = clientOptionSpawnLocations[counter].position;
                    attackPlayerOption.gameObject.SetActive(true);
                    counter++;
                }
            }

            if (token.creature.hasAbility) {
                if (token.creature.abilityCost <= clientCurrentMana) {
                    if (!token.castedAbility) {

                        abilityOption.transform.position = clientOptionSpawnLocations[counter].position;
                        abilityOption.gameObject.SetActive(true);
                        counter++;
                    }
                }
            }
        }



    }

    public void ArtifactOptionButton(ArtifactToken token) {

        if (isHost) {
            ResetAllTiles(hostBoard);
        } else {
            ResetAllTiles(clientBoard);
        }
        int counter = 0;
        abilityOption.token = token;
        token.GetComponent<Token>().ChangeMaterial(m_selected);

        attackTokenOption.gameObject.SetActive(false);
        attackPlayerOption.gameObject.SetActive(false);
        abilityOption.gameObject.SetActive(false);

        if (Singleton.isHost) {
            if (token.artifact.hasAbility) {
                if (token.artifact.abilityCost <= hostCurrentMana) {
                    if (!token.castedAbility) {
                        abilityOption.transform.position = hostOptionSpawnLocations[counter].position;
                        abilityOption.gameObject.SetActive(true);
                    }
                }
            }
        } else {
            if (token.artifact.hasAbility) {
                if (token.artifact.abilityCost <= clientCurrentMana) {
                    if (!token.castedAbility) {
                        abilityOption.transform.position = clientOptionSpawnLocations[counter].position;
                        abilityOption.gameObject.SetActive(true);
                    }
                }
            }
        }
    }

    IEnumerator SafeSceneChange(float time, string scene) {
        yield return new WaitForSeconds(time);
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene(scene);
    }


    public void AttackPlayerAnimation(int id, bool isHostTile) {
        if (isHostTile) {
            hostBoard[id].token.GetComponent<CreatureToken>().AttackPlayer();
        } else {
            clientBoard[id].token.GetComponent<CreatureToken>().AttackPlayer();
        }
    }

    public void OpponentDrawCard() {
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.UpdateDrawCardServerRpc(isHost);
    }

    public void OpponentHandAddCard(bool t) {
        if (isHost) {
            if (!t) {
                //if you are the host, and the drawn card was from the client...
                opponentHand.AddCardToHand();

            }
        } else {
            if (t) {
                //if you are the host, and the drawn card was from the client...
                opponentHand.AddCardToHand();

            }
        }
    }
    public void OpponentUsedCard() {
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.UpdateUseCardServerRpc(isHost);
    }

    public void OpponentHandRemoveCard(bool t) {
        if (isHost) {
            if (!t) {
                //if you are the host, and the drawn card was from the client...
                opponentHand.RemoveCardFromHand();

            }
        } else {
            if (t) {
                //if you are the host, and the drawn card was from the client...
                opponentHand.RemoveCardFromHand();

            }
        }
    }


    public void ChangeAllTileMaterials() {
        if (isHost) {
            for (int i = 0; i < hostBoard.Length; i++) {
                hostBoard[i].ChangeTileMaterial(m_default);
            }
        } else {
            for (int i = 0; i < clientBoard.Length; i++) {
                clientBoard[i].ChangeTileMaterial(m_default);
            }
        }
    }
    public void ShowAvailableTilesToPlaceCard(Card card) {
        if (isHost) {
            if (card is Creature c) {
                if (c.isMelee) {
                    for (int i = 5; i < 10; i++) {
                        if (hostBoard[i].token != null) {
                            continue;
                        } else {
                            hostBoard[i].ChangeTileMaterial(m_active);
                        }
                    }
                } else {
                    for (int i = 0; i < 5; i++) {
                        if (hostBoard[i].token != null) {
                            continue;
                        } else {
                            hostBoard[i].ChangeTileMaterial(m_active);
                        }
                    }
                }
            } else if (card is Artifact a) {
                for (int i = 0; i < 10; i++) {
                    if (hostBoard[i].token != null) {
                        continue;
                    } else {
                        hostBoard[i].ChangeTileMaterial(m_active);
                    }
                }
            }
        } else {
            if (card is Creature c) {
                if (c.isMelee) {
                    for (int i = 5; i < 10; i++) {
                        if (clientBoard[i].token != null) {
                            continue;
                        } else {
                            clientBoard[i].ChangeTileMaterial(m_active);
                        }
                    }
                } else {
                    for (int i = 0; i < 5; i++) {
                        if (clientBoard[i].token != null) {
                            continue;
                        } else {
                            clientBoard[i].ChangeTileMaterial(m_active);
                        }
                    }
                }
            } else if (card is Artifact a) {
                for (int i = 0; i < 10; i++) {
                    if (clientBoard[i].token != null) {
                        continue;
                    } else {
                        clientBoard[i].ChangeTileMaterial(m_active);
                    }

                }

            }
        }
    }


    public void TokenPlayParticles(int ID, bool hostTile) {

        if (hostTile) {
            Token t = hostBoard[ID].token.GetComponent<Token>();

            if (t is CreatureToken c) {
                c.PlayParticles();
            } else if (t is ArtifactToken a) {
                a.PlayParticles();
            }
        } else {
            Token t = clientBoard[ID].token.GetComponent<Token>();

            if (t is CreatureToken c) {
                c.PlayParticles();
            } else if (t is ArtifactToken a) {
                a.PlayParticles();
            }
        }
    }
}
