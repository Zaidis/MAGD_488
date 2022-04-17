using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Alvitr", fileName = "Card")]
public class Alvitr : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker,  bool isHost)
    {
        //Area attack does half attack to surround enemies - 2 mana
    }
}
