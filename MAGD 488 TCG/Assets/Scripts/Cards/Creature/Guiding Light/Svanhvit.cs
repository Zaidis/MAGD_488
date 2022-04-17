using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Svanhvit", fileName = "Card")]
public class Svanhvit : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Rage +1/-1 perm - 3mana
    }
}
