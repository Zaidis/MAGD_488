using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Lumine", fileName = "Card")]
public class Lumine : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //Heals 1 hp to target area, 6 targets max - 4mana
    }
}
