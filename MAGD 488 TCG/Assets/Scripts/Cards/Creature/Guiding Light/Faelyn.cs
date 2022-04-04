using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Faelyn", fileName = "Card")]
public class Faelyn : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //Copy Health and Attack of target creature
    }
}
