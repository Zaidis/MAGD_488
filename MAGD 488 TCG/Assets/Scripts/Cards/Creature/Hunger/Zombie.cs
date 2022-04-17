using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Zombie", fileName = "Card")]
public class Zombie : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Create zombie token in hand with 1/1/0 - 2 mana
    }
}
