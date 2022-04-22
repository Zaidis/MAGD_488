using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class CreatureToken : Token, IPointerClickHandler
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
        Name.text = creature.cardName;

        AttackText.text = currentAttack.ToString();
        HealthText.text = currentHealth.ToString();

        //Description.text = creature.description;
       // Mana.text = creature.manaCost.ToString();
      //  Art = creature.cardArt;
    }    

    public void UpdateStats() {
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
                    /*if (hasAttacked == false) {
                        //you can attack with this creature. 
                        //needs access to tile ID
                        //also needs to know if its melee or ranged
                        if (creature.isMelee) {
                            GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.clientBoard, true, transform.parent.GetComponent<Tile>().GetTileID());
                        }
                        else {
                            GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.clientBoard, false, transform.parent.GetComponent<Tile>().GetTileID());
                        }

                        GameManager.Singleton.isAttecking = true;
                        GameManager.Singleton.selectedCreature = this;
                    }*/
                }
                else {
                    if (GameManager.Singleton.isAttecking) {
                        if (transform.parent.GetComponent<Tile>().active) {
                            /*GameManager.Singleton.selectedCreature.AttackWithToken(transform.parent.GetComponent<Tile>());
                            GameManager.Singleton.selectedCreature = null;
                            GameManager.Singleton.isAttecking = false;*/

                            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                            p.Attack(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID());
                        }
                    }
                } 
            }
            else {
                //not the host 

            }
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            GameManager.Singleton.popup.UpdatePopup(creature);
        }

    }

    public void AttackWithToken(Tile attackedToken) {
        if (!hasAttacked) {
            creature.OnAttack(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard,
                    transform.parent.GetComponent<Tile>(), GameManager.Singleton.isHost, attackedToken);

            /* if (GameManager.Singleton.isHost) {
                 GameManager.Singleton.ResetAllTiles(GameManager.Singleton.clientBoard);
                 hasAttacked = true;
                 GameManager.Singleton.isAttecking = false;
                 GameManager.Singleton.selectedCreature = null;
             }*/

            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.hostBoard);
            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.clientBoard);
            hasAttacked = true;
            GameManager.Singleton.isAttecking = false;
            GameManager.Singleton.selectedCreature = null;
        }
    }
    
}
