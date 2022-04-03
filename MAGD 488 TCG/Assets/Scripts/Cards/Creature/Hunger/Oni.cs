using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Oni", fileName = "Card")]
public class Oni : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Vector2Int attacker, Vector2Int attacked, bool isHost)
    {
        //self-buff+surrounding ?
    }
}
