using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Babe", fileName = "Card")]
public class Babe : Creature
{
    [SerializeField] Card paulBunyon;
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
            if(c.currentHealth <= 0)
            {
                if (parent.hostTile)
                {
                    for (int i = 0; i < GameManager.Singleton.hostBoard.Length; i++)
                    {
                        if(GameManager.Singleton.hostBoard[i].token != null) {
                            if (GameManager.Singleton.hostBoard[i].token.GetComponent<Token>() is CreatureToken c2) {
                                if (c2.creature == paulBunyon) {
                                    c2.currentAttack += attackMod;
                                    c2.currentHealth += healthMod;
                                    c2.UpdateStats();
                                }
                            }
                        }
                       
                    }
                } else {
                    for (int i = 0; i < GameManager.Singleton.clientBoard.Length; i++)
                    {
                        if(GameManager.Singleton.clientBoard[i].token != null) {
                            if (GameManager.Singleton.clientBoard[i].token.GetComponent<Token>() is CreatureToken c2) {
                                if (c2.creature == paulBunyon) {
                                    c2.currentAttack += attackMod;
                                    c2.currentHealth += healthMod;
                                    c2.UpdateStats();
                                }
                            }
                        }
                       
                    }
                }
            }
        }
    }
}
