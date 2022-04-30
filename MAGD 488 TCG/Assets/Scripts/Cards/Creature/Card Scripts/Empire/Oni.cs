using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Oni", fileName = "Card")]
public class Oni : Creature
{
   // [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //self-buff+surrounding: +1/-1  -  2 mana cost
        if (GameManager.Singleton.isHost) {
            if (GameManager.Singleton.hostCurrentMana >= abilityCost) {
                int newMana = GameManager.Singleton.hostCurrentMana - abilityCost;

                GameManager.Singleton.AffectManaValues(newMana, GameManager.Singleton.clientCurrentMana,
                    GameManager.Singleton.hostMaxMana, GameManager.Singleton.clientMaxMana);
            }
            else {
                return;
            }
        }
        else {
            if (GameManager.Singleton.clientCurrentMana >= abilityCost) {
                int newMana = GameManager.Singleton.clientCurrentMana - abilityCost;

                GameManager.Singleton.AffectManaValues(GameManager.Singleton.hostCurrentMana, newMana,
                    GameManager.Singleton.hostMaxMana, GameManager.Singleton.clientMaxMana);
            }
            else {
                return;
            }
        }
        int myTileID = attacker.GetTileID();
        int leftTile = myTileID - 1;
        int rightTile = myTileID + 1;
        int frontTile = myTileID + 5;

        if (attacker.hostTile)
        {

            if (attacker.meleeTile)
            {
                //check left and right
                if (leftTile >= 5)
                {
                    if (hostBoard[leftTile].meleeTile)
                    {
                        if (hostBoard[leftTile].token != null)
                        {
                            if (hostBoard[leftTile].token.GetComponent<Token>() is CreatureToken c)
                            {
                                c.currentAttack += 1;
                                c.currentHealth -= 1;
                                c.UpdateStats();
                            }
                        }
                    }
                }
                if (rightTile <= 9)
                {
                    if (hostBoard[rightTile].meleeTile)
                    {
                        if (hostBoard[rightTile].token != null)
                        {
                            if (hostBoard[rightTile].token.GetComponent<Token>() is CreatureToken c)
                            {
                                c.currentAttack += 1;
                                c.currentHealth -= 1;
                                c.UpdateStats();
                            }
                        }
                    }
                }
            }
            else
            {
                //check in front
                if (hostBoard[frontTile].meleeTile)
                {
                    if (hostBoard[frontTile].token != null)
                    {
                        if (hostBoard[frontTile].token.GetComponent<Token>() is CreatureToken c)
                        {
                            c.currentAttack += 1;
                            c.currentHealth -= 1;
                            c.UpdateStats();
                        }
                    }
                }
            }

            attacker.token.GetComponent<Token>().UpdateStats();

        }
        else
        {
            if (attacker.meleeTile)
            {
                //check left and right
                if (leftTile >= 5)
                {
                    if (clientBoard[leftTile].meleeTile)
                    {
                        if (clientBoard[leftTile].token != null)
                        {
                            if (clientBoard[leftTile].token.GetComponent<Token>() is CreatureToken c)
                            {
                                c.currentAttack += 1;
                                c.currentHealth -= 1;
                                c.UpdateStats();
                            }
                        }
                    }
                }
                if (rightTile <= 9)
                {
                    if (clientBoard[rightTile].meleeTile)
                    {
                        if (clientBoard[rightTile].token != null)
                        {
                            if (clientBoard[rightTile].token.GetComponent<Token>() is CreatureToken c)
                            {
                                c.currentAttack += 1;
                                c.currentHealth -= 1;
                                c.UpdateStats();
                            }
                        }
                    }
                }
            }
            else
            {
                //check in front
                if (clientBoard[frontTile].meleeTile)
                {
                    if (clientBoard[frontTile].token != null)
                    {
                        if (clientBoard[frontTile].token.GetComponent<Token>() is CreatureToken c)
                        {
                            c.currentAttack += 1;
                            c.currentHealth -= 1;
                            c.UpdateStats();
                        }
                    }
                }
            }

            attacker.token.GetComponent<Token>().UpdateStats();
        }
        
    }
}
