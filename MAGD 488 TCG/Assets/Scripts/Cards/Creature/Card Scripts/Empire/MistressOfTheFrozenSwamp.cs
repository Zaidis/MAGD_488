using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/MistressOfTheFrozenSwamp")]
public class MistressOfTheFrozenSwamp : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker,  bool isHost)
    {
        //buff/big buff if hunger creature - 3 mana cost
    }
}
