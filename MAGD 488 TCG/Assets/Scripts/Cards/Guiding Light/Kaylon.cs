using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/Kaylon", fileName = "Card")]
public class Kaylon : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Special Effect Arrows(fire / burn, etc)(4 - 5 mana cost)
        //Send animal to attack(can send hawk for range attack or boar to attack melee) (4 - 5 mana cost) (send individual animal hawk = 3 mana boar = 4) (or set one animal)
    }
}
