using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/leader", fileName = "Card")]
public class leader : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //Area attack does half attack to surround enemies - 2 mana
    }
}
