using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/Witch")]
public class Witch : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //One card, buff to everyone, and big buff to hunger creature
    }
}
