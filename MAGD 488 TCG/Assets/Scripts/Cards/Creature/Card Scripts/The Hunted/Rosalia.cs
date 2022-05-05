using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Rosalia", fileName = "Card")]
public class Rosalia : Creature
{
    public int attackMod;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }


    public override void OnAttacked(Tile parent) {
        if (parent.token != null) {
            Token t = parent.token.GetComponent<Token>();
            if (t is CreatureToken c) {
                if (c.currentHealth < c.maxHealth) {
                    c.currentAttack += attackMod;
                    c.UpdateStats();
                }
            }
        }

    }

}
