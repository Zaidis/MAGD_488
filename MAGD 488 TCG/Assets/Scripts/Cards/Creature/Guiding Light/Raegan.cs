using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Raegan", fileName = "Card")]
public class Raegan : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //can make enemy flee, (return to deck) (only those with less than or equal to mana cost) - 4 mana
    }
}
