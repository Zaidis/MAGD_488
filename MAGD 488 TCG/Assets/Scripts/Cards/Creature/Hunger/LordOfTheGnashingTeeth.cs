using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/LordOfTheGnashingTeeth", fileName = "Card")]
public class LordOfTheGnashingTeeth : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //self-buff add thorns for the next 2 turns + -2/0 - 2 mana cost
    }
}
