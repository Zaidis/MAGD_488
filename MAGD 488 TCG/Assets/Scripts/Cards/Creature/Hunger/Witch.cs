using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/Witch")]
public class Witch : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //buff/big buff if hunger creature - 3 mana cost
    }
}
