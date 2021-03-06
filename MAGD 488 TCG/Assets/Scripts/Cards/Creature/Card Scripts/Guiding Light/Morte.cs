using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Morte", fileName = "Card")]
public class Morte : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker,  bool isHost)
    {
        //heal 1 hp to any card on board or player - 3 mana
        //Apply Poison to enemy directly ahead of placement and in 2 rounds lose 1 health - 3 mana
    }
}
