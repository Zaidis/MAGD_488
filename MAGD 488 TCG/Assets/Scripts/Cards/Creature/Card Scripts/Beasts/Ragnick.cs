using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Ragnick", fileName = "Card")]
public class Ragnick : Creature
{
    [SerializeField] int abilityCost, healthIncrease;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        if (GameManager.Singleton.currentMana >= abilityCost)
        {
            GameManager.Singleton.AffectCurrentMana(-abilityCost);
        }
        else
        {
            return;
        }
        Token t = attacker.token.GetComponent<Token>();
        if(t is CreatureToken c)
        {
            c.maxHealth += healthIncrease;
        }
    }
}
