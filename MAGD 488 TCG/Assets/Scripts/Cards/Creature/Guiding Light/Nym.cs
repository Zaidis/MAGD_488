using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Nym", fileName = "Card")]
public class Nym : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //All friendly ranged creatures +1/+1
    }
}
