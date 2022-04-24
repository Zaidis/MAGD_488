using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Faelyn", fileName = "Card")]
public class Faelyn : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker,  bool isHost)
    {
        //Copy Health and Attack of target creature
    }
}
