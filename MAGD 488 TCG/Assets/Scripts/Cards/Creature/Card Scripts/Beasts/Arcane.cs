using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Arcane", fileName = "Card")]
public class Arcane : Creature
{
    [SerializeField] Card momo, tauro, celestial;
    [SerializeField] int attackMod, healthMod;

    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }

    public override void OnAttacked(Tile parent)
    {
        Token t = parent.token.GetComponent<Token>();
        if(t is CreatureToken c)
        {
            if(c.currentHealth < c.maxHealth)
            {
                c.currentAttack += attackMod;
                c.currentHealth += healthMod;
                if(!c.myAttributes.Contains(attributes.pierce))
                    c.myAttributes.Add(attributes.pierce);
                c.UpdateStats();
            }
        }
    }
    public override void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent)
    {
        if (parent.hostTile)
        {
            if (DoLoop(hostBoard))
            {
                GameManager.Singleton.myHand.AddCardToHand(celestial);
            }
        }
        else
        {
            if (DoLoop(clientBoard))
            {
                GameManager.Singleton.myHand.AddCardToHand(celestial);
            }
        }
    }
    bool DoLoop(Tile[] board)
    {
        bool tFound = false, aFound = false;
        for (int i = 0; i < board.Length; i++)
        {
            if (board[i].token.GetComponent<Token>() is CreatureToken c)
            {
                if (c.creature == momo)
                {
                    tFound = true;
                }
                else if (c.creature == tauro)
                {
                    aFound = true;
                }
            }
        }

        if (tFound && aFound)
            return true;
        else
            return false;
    }
}
