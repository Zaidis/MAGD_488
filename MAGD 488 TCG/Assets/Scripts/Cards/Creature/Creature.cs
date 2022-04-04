using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature/Default", fileName = "Card")]
public class Creature : Card
{ 
    [Header("Creature Info")]
    public int defaultHealthAmount;
    public int defaultPowerAmount; //attack damage
    public List<attributes> myAttributes;

    public virtual void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {

    }
    public virtual void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {

    }
}
