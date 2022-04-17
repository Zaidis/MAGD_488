using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Faelyn", fileName = "Card")]
public class Faelyn : Creature
{
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker,  bool isHost)
    {
        //Copy Health and Attack of target creature
    }
}
