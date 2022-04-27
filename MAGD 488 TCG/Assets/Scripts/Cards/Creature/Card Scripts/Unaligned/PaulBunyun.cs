using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/PaulBunyun", fileName = "Card")]
public class PaulBunyun : Creature
{
    [SerializeField] Card babe;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
       
    }

    public override void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent)
    {
        if (GameManager.Singleton.isHost) {
            if (parent.hostTile) {
                GameManager.Singleton.myHand.AddCardToHand(babe);
            }
        } else {
            if (!parent.hostTile) {
                GameManager.Singleton.myHand.AddCardToHand(babe);
            }
        }
        
    }
}
