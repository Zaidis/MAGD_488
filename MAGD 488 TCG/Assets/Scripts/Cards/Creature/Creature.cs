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

        /*if (isHost) {
            if (attacker.token.GetComponent<CreatureToken>().creature.isMelee) {
                //if the creature is melee, it attacks in front if itself

                //GameManager.Singleton.ChangeTilesMaterial(clientBoard, true);

                if (clientBoard[attacker.GetTileID()].token != null) { //check if there is a token here
                    //first the client's creautre is attacked
                    clientBoard[attacker.GetTileID()].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);

                    //second the attacker is dealt damage 
                    attacker.DealtDamage(clientBoard[attacker.GetTileID()].token.GetComponent<CreatureToken>().currentAttack);
                } else if (clientBoard[attacker.GetTileID() - 5].token != null) { //check if the tile behind it is not null
                    //we attack the next tile behind the ID's tile. 
                    clientBoard[attacker.GetTileID() - 5].DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
                    attacker.DealtDamage(clientBoard[attacker.GetTileID() - 5].token.GetComponent<CreatureToken>().currentAttack);
                } else {
                    //attack the player!
                    Debug.Log("I hit the player for " + attacker.token.GetComponent<CreatureToken>().currentAttack);
                }
            } else {
                //this is ranged
            }
        } */

        //first attacker will hit the attecked token
        attacked.DealtDamage(attacker.token.GetComponent<CreatureToken>().currentAttack);
        attacker.DealtDamage(attacked.token.GetComponent<CreatureToken>().currentAttack);

    }
    public virtual void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }
}
