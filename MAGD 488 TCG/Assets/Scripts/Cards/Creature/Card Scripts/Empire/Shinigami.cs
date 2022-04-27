using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/Shinigami")]
public class Shinigami : Creature
{
    [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Lifesteal 3 Mana, Heals player equal to attack

        if (GameManager.Singleton.currentMana >= abilityCost)
        {
            GameManager.Singleton.AffectCurrentMana(-abilityCost);
        }
        else
        {
            return;
        }

        if (attacker.hostTile)
        {
            Token t = attacker.token.GetComponent<Token>();
            if(t is CreatureToken c)
            {
                GameManager.Singleton.AffectHealthValues(c.currentAttack, 0);
            }

        }
        else
        {
            Token t = attacker.token.GetComponent<Token>();
            if (t is CreatureToken c)
            {
                GameManager.Singleton.AffectHealthValues(0, c.currentAttack);
            }
        }
    }
}