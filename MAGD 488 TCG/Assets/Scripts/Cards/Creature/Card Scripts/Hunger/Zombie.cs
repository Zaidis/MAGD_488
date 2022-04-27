using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Zombie", fileName = "Card")]
public class Zombie : Creature
{
    public Creature zombieToken;
    [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Create zombie token in hand with 1/1/0 - 2 mana
        if(GameManager.Singleton.currentMana >= abilityCost)
        {
            GameManager.Singleton.myHand.AddCardToHand(zombieToken);
            GameManager.Singleton.AffectCurrentMana(-abilityCost);
        }
            
    }
}
