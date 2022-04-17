using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Olrun", fileName = "Card")]
public class Olrun : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Applies dizzy to enemy, has a chance to render unusable next turn - 4 mana
    }
}
