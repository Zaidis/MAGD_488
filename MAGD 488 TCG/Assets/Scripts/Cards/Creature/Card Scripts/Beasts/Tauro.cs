using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Tauro", fileName = "Card")]
public class Tauro : Creature
{
    [SerializeField] Card momo, arcane, celestial;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

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
                else if (c.creature == arcane)
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
