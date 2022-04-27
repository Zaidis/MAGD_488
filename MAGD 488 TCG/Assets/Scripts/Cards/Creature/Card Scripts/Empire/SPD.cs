using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/SPD", fileName = "Card")]
public class SPD : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Debuff Enemy card, fear target card - 2mana cost
    }

    public override void OnTargetedAbility(Tile user, Tile victim, bool isHostSide) {
        //base.OnTargetedAbility(user, victim, isHostSide);

        if(victim.token.GetComponent<Token>() is CreatureToken c) {
            c.hasAttacked = true;
        }
        

    }
}