using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Ashryn", fileName = "Card")]
public class Ashryn : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Self buff +1/+1 permanant - 2 mana
    }
}
