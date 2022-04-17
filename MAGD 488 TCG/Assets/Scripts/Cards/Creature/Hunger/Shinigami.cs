using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/Shinigami")]
public class Shinigami : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Lifesteal 3 Mana, Heals player equal to attack
    }
}