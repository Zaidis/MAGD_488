using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Nightmare Demon", fileName = "Card")]
public class NightmareDemon : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //self-buff add thorns for the next 2 turns
    }
}
