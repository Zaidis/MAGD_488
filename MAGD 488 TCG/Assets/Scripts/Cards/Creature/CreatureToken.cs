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


    public override void ApplyCard() {
        currentAttack = creature.defaultPowerAmount;
        currentHealth = creature.defaultHealthAmount;
       // Name.text = creature.cardName;

        AttackText.text = currentAttack.ToString();
        HealthText.text = currentHealth.ToString();

        //Description.text = creature.description;
       // Mana.text = creature.manaCost.ToString();
        Art = creature.cardArt;
    }

    public override void OnPlay() {
        //throw new System.NotImplementedException();
    }
    public override void UpdateStats() {
        //health
        if(currentHealth > creature.defaultHealthAmount) {
            //green text because its bigger
            HealthText.color = Color.green;
        } else if (currentHealth == creature.defaultHealthAmount) {
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
            if (GameManager.Singleton.isHost) {
                if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.hostBoard, transform.parent.GetComponent<Tile>())) {
                    //this is my creature

                    GameManager.Singleton.CreatureOptionButtons(this, GameManager.Singleton.isHost);
                   
                }
                else {
                    if (GameManager.Singleton.isAttecking) {
                        if (transform.parent.GetComponent<Tile>().active) {
                            /*GameManager.Singleton.selectedCreature.AttackWithToken(transform.parent.GetComponent<Tile>());
                            GameManager.Singleton.selectedCreature = null;
                            GameManager.Singleton.isAttecking = false;*/

                            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                            p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), true);
                        }
                    }
                } 
            }
            else {
                //not the host 
                if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.clientBoard, transform.parent.GetComponent<Tile>())) {
                    //this is my creature

                    GameManager.Singleton.CreatureOptionButtons(this, false);

                }
                else {
                    if (GameManager.Singleton.isAttecking) {
                        if (transform.parent.GetComponent<Tile>().active) {
                            /*GameManager.Singleton.selectedCreature.AttackWithToken(transform.parent.GetComponent<Tile>());
                            GameManager.Singleton.selectedCreature = null;
                            GameManager.Singleton.isAttecking = false;*/

                            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                            p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), false);
                        }
                    }
                }
            }
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            GameManager.Singleton.panelPopup.UpdatePopup(creature);
        }

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
            hasAttacked = true;
            GameManager.Singleton.isAttecking = false;
            GameManager.Singleton.selectedCreature = null;
        }
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
}
