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
    [SerializeField] private bool hasAttacked;


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
        if (GameManager.Singleton.isHost) {
            if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.hostBoard, transform.parent.GetComponent<Tile>())) {
                //this is my creature
                if (hasAttacked == false) {
                    //you can attack with this creature. 
                    //needs access to tile ID
                    //also needs to know if its melee or ranged
                    if (creature.isMelee) {
                        GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.clientBoard, true);
                    } else {
                        GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.clientBoard, false);
                    }

                    GameManager.Singleton.isAttecking = true;
                    GameManager.Singleton.selectedCreature = this;
                }
            } else {
                if (GameManager.Singleton.isAttecking) {
                    if (transform.parent.GetComponent<Tile>().active) {
                        GameManager.Singleton.selectedCreature.AttackWithToken(transform.parent.GetComponent<Tile>());
                        GameManager.Singleton.selectedCreature = null;
                        GameManager.Singleton.isAttecking = false;
                    }
                }
            }
        } else {
            //not the host
            
        }
        

    }

    public void AttackWithToken(Tile attackedToken) {
        creature.OnAttack(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard,
                transform.parent.GetComponent<Tile>(), GameManager.Singleton.isHost, attackedToken);

        if (GameManager.Singleton.isHost) {
            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.clientBoard);
            hasAttacked = true;
            GameManager.Singleton.isAttecking = false;
            GameManager.Singleton.selectedCreature = null;
        }
        
    }
    
}
