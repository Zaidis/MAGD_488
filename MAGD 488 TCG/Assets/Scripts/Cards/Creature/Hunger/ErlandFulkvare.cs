using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/ErlandFulkvare", fileName = "Card")]
public class ErlandFulkvare : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //All cards,0/+2 and Taunt (4 Mana Cost)
    }
}
