using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/BlackKnight", fileName = "Card")]
public class BlackKnight : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //self buff, either pierce of cleave, 4 mana cost, choose every turn
    }
}