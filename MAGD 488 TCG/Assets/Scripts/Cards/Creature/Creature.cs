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

    public virtual void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {

        //first attacker will hit the attecked token
        attacker.token.GetComponent<CreatureToken>().hasAttacked = true;
        GameManager.Singleton.CreatureOptionButtons(attacker.token.GetComponent<CreatureToken>(), GameManager.Singleton.isHost);

        attacked.DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);

        if (attacked.token.GetComponent<Token>() is CreatureToken c) {
            attacker.DealtDamage(c.currentAttack);
        }

        //Attributes

        if (myAttributes.Contains(attributes.cleave)) {
            Cleave(hostBoard, clientBoard, attacker, isHost, attacked);
        }
        if (myAttributes.Contains(attributes.pierce)) {
            Pierce(hostBoard, clientBoard, attacker, isHost, attacked);
        }
        if (myAttributes.Contains(attributes.lifesteal)) { //when this deals damage, heal your player
            LifeSteal(attacker, attacked);
        }
        if (myAttributes.Contains(attributes.taunt)) { //must be destroyed before attacking another tile

        }
    }

    public virtual void OnPlay(Tile[] hostBoard, Tile clientBoard) {
        //do nothing normally
    }

    public virtual void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

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
        if (isHost) {
            if(clientBoard[attackedID - 1] != null) { //check the tile to the left of the attacked tile
                if(clientBoard[attackedID - 1].token != null) { //if there is a token here, attack it
                    clientBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
            if (clientBoard[attackedID + 1] != null) { //check the tile to the right of the attacked tile
                if (clientBoard[attackedID + 1].token != null) {
                    clientBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
        } else {
            if (hostBoard[attackedID - 1] != null) {
                if (hostBoard[attackedID - 1].token != null) {
                    hostBoard[attackedID - 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
            if (hostBoard[attackedID + 1] != null) {
                if (hostBoard[attackedID + 1].token != null) {
                    hostBoard[attackedID + 1].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
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

        if (isHost) {
            if(clientBoard[attackedID - 5] != null) {
                if(clientBoard[attackedID - 5].token != null) {
                    clientBoard[attackedID - 5].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
        } else {
            if (hostBoard[attackedID - 5] != null) {
                if (hostBoard[attackedID - 5].token != null) {
                    hostBoard[attackedID - 5].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            }
        }
    }

    public void LifeSteal(Tile attacker, Tile attacked) {

        Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        if (attacker.hostTile) {
            p.UpdateHealthServerRpc(attacker.token.GetComponent<CreatureToken>().currentAttack, 0);
        } else {
            p.UpdateHealthServerRpc(0, attacker.token.GetComponent<CreatureToken>().currentAttack);
        }
        
    }
}
