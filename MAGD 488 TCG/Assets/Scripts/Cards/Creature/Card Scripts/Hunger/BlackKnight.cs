using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/BlackKnight", fileName = "Card")]
public class BlackKnight : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked) {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
        Cleave(hostBoard, clientBoard, attacker, isHost, attacked);
        Pierce(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //self buff, either pierce of cleave, 4 mana cost, choose every turn
    }
}