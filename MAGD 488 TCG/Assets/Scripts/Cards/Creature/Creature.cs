using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/Default", fileName = "Card")]
public class Creature : Card
{ 
    [Header("Creature Info")]
    public int defaultHealthAmount;
    public int defaultPowerAmount; //attack damage
    public List<attributes> myAttributes;
    public bool isMelee;
    public bool hasAbility;
    public bool hasTargetedAbility;
    public bool targetFriendly;
    public bool targetEnemy;
    public int abilityCost;
    public virtual void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {

        //first attacker will hit the attecked token
        attacker.token.GetComponent<CreatureToken>().hasAttacked = true;
        //GameManager.Singleton.CreatureOptionButtons(attacker.token.GetComponent<CreatureToken>(), GameManager.Singleton.isHost);
        GameManager.Singleton.TurnOffOptionsAndUnselect();
        attacked.DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
        

        if (attacked.token.GetComponent<Token>() is CreatureToken c) {
            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
            if ((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {

                if (c.myAttributes.Contains(attributes.thorn)) { //when attacked, deals damage to attacker
                    if (attacker.hostTile) { //deals 1 damage to the opponent
                        Debug.LogError("THORNS -> HOST AFFECTED");
                        p.UpdateHealthServerRpc(c.currentAttack * -1, 0);
                    }
                    else { //client health goes up
                        Debug.LogError("THORNS -> CLIENT AFFECTED");
                        p.UpdateHealthServerRpc(0, c.currentAttack * -1);
                    }
                }
            }


            attacker.DealtDamage(c.currentAttack);
        }

        //Attributes

        if (attacker.token.GetComponent<CreatureToken>().myAttributes.Contains(attributes.cleave)) {
            Cleave(hostBoard, clientBoard, attacker, isHost, attacked);
        }
        if (attacker.token.GetComponent<CreatureToken>().myAttributes.Contains(attributes.pierce)) {
            Pierce(hostBoard, clientBoard, attacker, isHost, attacked);
        }
        if (attacker.token.GetComponent<CreatureToken>().myAttributes.Contains(attributes.lifesteal)) { //when this deals damage, heal your player
            LifeSteal(attacker, attacked);
        }
        if (attacker.token.GetComponent<CreatureToken>().myAttributes.Contains(attributes.taunt)) { //must be destroyed before attacking another tile

        } 
    }

    public virtual void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent) {
        //do nothing normally
    }

    public virtual void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {



    }
    
    public virtual void OnTargetedAbility(Tile user, Tile victim, bool isHostSide) {



    }

    public virtual void OnAttacked(Tile parent)
    {
        //do nothing normally
    }

    /// <summary>
    /// Attacker deals damage to the left and right of the attacked token. 
    /// </summary>
    /// <param name="hostBoard"></param>
    /// <param name="clientBoard"></param>
    /// <param name="attacker"></param>
    /// <param name="isHost"></param>
    /// <param name="attacked"></param>
    public void Cleave(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked) {

        int attackedID = attacked.token.transform.parent.GetComponent<Tile>().GetTileID();

        if (!attacked.token.GetComponentInParent<Tile>().hostTile) {

            bool isMelee = clientBoard[attackedID].meleeTile;

            
            
            if(attackedID - 1 >= 0 && attackedID - 1 <= 9) { //check the tile to the left of the attacked tile
                if(clientBoard[attackedID - 1].token != null) { //if there is a token here, attack it
                    if(clientBoard[attackedID - 1].meleeTile && isMelee)
                        clientBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                    else if(clientBoard[attackedID - 1].meleeTile == false && !isMelee)
                        clientBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
            if(attackedID + 1 >= 0 && attackedID + 1 <= 9) {
                if (clientBoard[attackedID + 1].token != null) { //if there is a token here, attack it
                    if (clientBoard[attackedID + 1].meleeTile && isMelee)
                        clientBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                    else if (clientBoard[attackedID + 1].meleeTile == false && !isMelee)
                        clientBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
        } else {
            if (attackedID - 1 >= 0 && attackedID - 1 <= 9) { //check the tile to the left of the attacked tile
                if (hostBoard[attackedID - 1].token != null) { //if there is a token here, attack it
                    if (hostBoard[attackedID - 1].meleeTile && isMelee)
                        hostBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                    else if (hostBoard[attackedID - 1].meleeTile == false && !isMelee)
                        hostBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
            if (attackedID + 1 >= 0 && attackedID + 1 <= 9) {
                if (hostBoard[attackedID + 1].token != null) { //if there is a token here, attack it
                    if (hostBoard[attackedID + 1].meleeTile && isMelee)
                        hostBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                    else if (hostBoard[attackedID + 1].meleeTile == false && !isMelee)
                        hostBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }

            /*if (hostBoard[attackedID - 1] != null) {
                if (hostBoard[attackedID - 1].token != null) {
                    hostBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
            if (hostBoard[attackedID + 1] != null) {
                if (hostBoard[attackedID + 1].token != null) {
                    hostBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }*/
        }
    }

    /// <summary>
    /// Attacker hits behind the attacked tile. 
    /// </summary>
    /// <param name="hostBoard"></param>
    /// <param name="clientBoard"></param>
    /// <param name="attacker"></param>
    /// <param name="isHost"></param>
    /// <param name="attacked"></param>
    public void Pierce(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked) {
        int attackedID = attacked.token.transform.parent.GetComponent<Tile>().GetTileID();

        if (!attacked.token.GetComponentInParent<Tile>().hostTile) {
            if (clientBoard[attackedID].meleeTile) {
                if (clientBoard[attackedID - 5].token != null) {
                    clientBoard[attackedID - 5].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
        } else {
            if (hostBoard[attackedID].meleeTile) {
                if (hostBoard[attackedID - 5].token != null) {
                    hostBoard[attackedID - 5].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
        }
    }

    public void LifeSteal(Tile attacker, Tile attacked) {
        Debug.Log("Gaining Life!!!");

        Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        if (GameManager.Singleton.isHost) {
            if (attacker.hostTile) {
                p.UpdateHealthServerRpc(attacker.token.GetComponent<CreatureToken>().currentAttack, 0);
            }
        } else {
            if (!attacker.hostTile) {
                p.UpdateHealthServerRpc(0, attacker.token.GetComponent<CreatureToken>().currentAttack);
            }
        }
         
        
    }
}
