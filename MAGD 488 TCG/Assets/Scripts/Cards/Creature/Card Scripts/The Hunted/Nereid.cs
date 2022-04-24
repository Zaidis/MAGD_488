using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Nereid", fileName = "Card")]
public class Nereid : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }
}