using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Nym", fileName = "Card")]
public class Nym : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //All friendly ranged creatures +1/+1
    }
}
