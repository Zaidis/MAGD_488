using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/ErlandFulkvare", fileName = "Card")]
public class ErlandFulkvare : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //All cards,0/+2 and Taunt (4 Mana Cost)
    }
}
