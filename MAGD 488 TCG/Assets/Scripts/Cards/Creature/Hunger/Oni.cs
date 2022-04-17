using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Oni", fileName = "Card")]
public class Oni : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //self-buff+surrounding: +1/-1  -  2 mana cost
    }
}
