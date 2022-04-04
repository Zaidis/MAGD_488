using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Svanhvit", fileName = "Card")]
public class Svanhvit : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //Rage +1/-1 perm - 3mana
    }
}
