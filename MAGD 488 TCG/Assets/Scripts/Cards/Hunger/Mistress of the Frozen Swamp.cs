using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/MistressOfTheFrozenSwamp", fileName = "Card")]
public class MistressOfTheFrozenSwamp : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //One Card: Buff to everyone & Big Buff to Hunger Creature (3 mana cost)
        //Two Card: Big Buff to a single creature(3 mana cost)
    }
}
