using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/SPD", fileName = "Card")]
public class SPD : Creature
{ 
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //Debuff Enemy card, fear
    }
}