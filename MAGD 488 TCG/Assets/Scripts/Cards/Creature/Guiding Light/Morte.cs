using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Morte", fileName = "Card")]
public class Morte : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //heal 1 hp to any card on board or player - 3 mana
        //Apply Poison to enemy directly ahead of placement and in 2 rounds lose 1 health - 3 mana
    }
}
