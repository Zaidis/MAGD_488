using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CreatureToken : Token, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Creature Token Variables")]
    public int currentAttack;
    //public int currentHealth;
    public Creature creature;
    public TextMeshPro AttackText;
    public TextMeshPro HealthText;
    public bool hasAttacked;

    public bool hasAbility;
    public bool castedAbility;
    public int maxHealth;
    public List<attributes> myAttributes = new List<attributes>();

    public override void ApplyCard() {
        maxHealth = creature.defaultHealthAmount;
        currentAttack = creature.defaultPowerAmount;
        currentHealth = maxHealth;

        myAttributes = creature.myAttributes;
       // Name.text = creature.cardName;

        AttackText.text = currentAttack.ToString();
        HealthText.text = currentHealth.ToString();

        //Description.text = creature.description;
       // Mana.text = creature.manaCost.ToString();
        Art = creature.cardArt;
    }


    public void ResetToken() {
        hasAttacked = false;
        castedAbility = false;
    }

    public override void OnPlay() {
        creature.OnPlay(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard,
            transform.parent.GetComponent<Tile>());
    }
    public override void UpdateStats() {
        //health
        if(currentHealth > maxHealth) {
            //green text because its bigger
            HealthText.color = Color.green;
        } else if (currentHealth == maxHealth) {
            HealthText.color = Color.white;
        } else {
            HealthText.color = Color.red;
        }

        //attack
        if(currentAttack > creature.defaultPowerAmount) {
            AttackText.color = Color.green;
        } else if (currentAttack == creature.defaultPowerAmount) {
            AttackText.color = Color.white;
        } else {
            AttackText.color = Color.red;
        }

        HealthText.text = currentHealth.ToString();
        AttackText.text = currentAttack.ToString();
    }
    
    /// <summary>
    /// Called when you want to attack with this creature. 
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerClick(PointerEventData eventData) {
        
        if (eventData.button == PointerEventData.InputButton.Left) {
            if((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {
                if (transform.parent.GetComponent<Tile>().active) {
                    if (GameManager.Singleton.isAttecking) {

                        if (GameManager.Singleton.isHost) {
                            if (!transform.GetComponentInParent<Tile>().hostTile) { //you clicked on a client tile
                                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                                p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), true);

                            }

                        }
                        else {
                            if (transform.GetComponentInParent<Tile>().hostTile) { //you clicked on a host tile
                                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                                p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), false);

                            }
                        }

                    }
                    else if (GameManager.Singleton.isUsingAbility) {

                        Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                        p.UpdateTargetedAbilityServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(),
                            transform.parent.GetComponent<Tile>().hostTile);

                    }


                }
                else { //NOT AN ACTIVE TILE

                    if (GameManager.Singleton.isHost) {
                        if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.hostBoard, transform.parent.GetComponent<Tile>())) {
                            //this is my creature

                            GameManager.Singleton.CreatureOptionButtons(this, GameManager.Singleton.isHost);
                        }
                    }
                    else {
                        if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.clientBoard, transform.parent.GetComponent<Tile>())) {
                            //this is my creature

                            GameManager.Singleton.CreatureOptionButtons(this, GameManager.Singleton.isHost);

                        }
                    }


                }
            }
            

           
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            GameManager.Singleton.panelPopup.UpdatePopup(creature);
        }

    }

    public void PlayParticles() {
        Debug.Log("Playing particles");
        particles.Play();
    }

    public void UseAbility() {
        creature.OnAbility(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard, transform.parent.GetComponent<Tile>(), GameManager.Singleton.isHost);
        castedAbility = true;

       // GameManager.Singleton.TurnOffOptionsAndUnselect();
        GameManager.Singleton.CreatureOptionButtons(this, GameManager.Singleton.isHost);


    }

    public void UseTargetedAbility(Tile targetedToken) {
        if (!castedAbility) {
            creature.OnTargetedAbility(transform.parent.GetComponent<Tile>(), targetedToken, transform.parent.GetComponent<Tile>().hostTile);
            castedAbility = true;
            
            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.hostBoard);
            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.clientBoard);

            //GameManager.Singleton.TurnOffOptionsAndUnselect();
            GameManager.Singleton.CreatureOptionButtons(this, GameManager.Singleton.isHost);
        }
        
    }

    public void OnAttacked()
    {
        creature.OnAttacked(transform.parent.GetComponent<Tile>());
    }
    public void AttackWithToken(Tile attackedToken) {
        if (!hasAttacked) {
            

            //animation

            if (transform.parent.GetComponent<Tile>().hostTile) {
                BeginMovement(attackedToken.AttackLocation.localPosition, attackedToken);
            } else {
                BeginMovement(attackedToken.AttackLocation.localPosition, attackedToken);
            }

            

            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.hostBoard);
            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.clientBoard);
            //GameManager.Singleton.TurnOffOptionsAndUnselect();
           // hasAttacked = true;
            GameManager.Singleton.isAttecking = false;
            GameManager.Singleton.selectedCreature = null;
        }
    }

    /// <summary>
    /// Called for animation
    /// </summary>
    public void AttackPlayer() {

        
         StartCoroutine(MoveUp(transform.localPosition, new Vector3(transform.localPosition.x, 0.006f, transform.localPosition.z)));
      
        

    }
    
    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.Singleton.cardPopup.UpdateHoverPopup(creature);
        GameManager.Singleton.cardPopup.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.Singleton.cardPopup.gameObject.SetActive(false);
    }

    private void BeginMovement(Vector3 attackedPosition, Tile a) {
        Debug.Log("Moving...");
        StartCoroutine(MoveUp(transform.localPosition, new Vector3(transform.localPosition.x, 0.006f, transform.localPosition.z), attackedPosition, a));
    }


    private IEnumerator MoveUp(Vector3 startPosition, Vector3 endPosition) {
        float speed = 2f;
        float y = 0.004f;

        var i = 0f;
        //var rate = 1f / 2f;
        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }

        if (transform.GetComponentInParent<Tile>().hostTile) {
            StartCoroutine(MoveTowardsGem(endPosition, GameManager.Singleton.clientHealthGemPosition.localPosition));
        } else {
            StartCoroutine(MoveTowardsGem(endPosition, GameManager.Singleton.hostHealthGemPosition.localPosition));
        }
            
    }

    private IEnumerator MoveTowardsGem(Vector3 startLocation, Vector3 targetLocation) {

        float speed = 4f;
        var i = 0f;


        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startLocation, targetLocation, i);
            yield return null;
        }

        StartCoroutine(MoveBack(startLocation, targetLocation));

    }

    // ---------------------
    #region ANIMATION FOR ATTACKING A TOKEN
    private IEnumerator MoveUp(Vector3 startPosition, Vector3 endPosition, Vector3 loc, Tile a) {
        float speed = 2f;
        float y = 0.004f;

        var i = 0f;
        //var rate = 1f / 2f;
        while(i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }


        StartCoroutine(MoveTowardsToken(loc, endPosition, a));
    }

    private IEnumerator MoveTowardsToken(Vector3 targetLocation, Vector3 startLocation, Tile a) {

        float speed = 4f;
        var i = 0f;


        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startLocation, targetLocation, i);
            yield return null;
        }

        //at this point, the token has struct the other token
        if(hasAttacked == false)
            creature.OnAttack(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard,
                     transform.parent.GetComponent<Tile>(), GameManager.Singleton.isHost, a);


        StartCoroutine(MoveBack(startLocation, targetLocation));

    }

    private IEnumerator MoveBack(Vector3 targetLocation, Vector3 startLocation) {
        float speed = 4f;
        var i = 0f;


        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startLocation, targetLocation, i);
            yield return null;
        }

        StartCoroutine(MoveDown(transform.localPosition, new Vector3(transform.localPosition.x, 
            transform.parent.GetComponent<Tile>().AttackLocation.transform.localPosition.y, transform.localPosition.z)));
    }

    private IEnumerator MoveDown(Vector3 startPosition, Vector3 endPosition) {
        float speed = 2f;
        float y = 0.004f;

        var i = 0f;
        //var rate = 1f / 2f;
        while (i < 1f) {
            i += Time.deltaTime * speed;
            transform.localPosition = Vector3.Lerp(startPosition, endPosition, i);
            yield return null;
        }
    }
    #endregion
}
