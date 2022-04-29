using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/ErlandFulkvare", fileName = "Card")]
public class ErlandFulkvare : Creature
{
    [SerializeField] int abilityCost, healthMod;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
        
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //All cards,0/+2 and Taunt (4 Mana Cost)
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
        //Token t = attacker.token.GetComponent<Token>();
        if (isHost)
        {
            for (int i = 0; i < 10; i++)
            {
                Tile tile = hostBoard[i];
                if(tile.token != null) {
                    Token t = tile.token.GetComponent<Token>();
                    if (t is CreatureToken c) {
                        c.currentHealth += healthMod;
                        c.UpdateStats();
                    }
                }
                
            }
        } else
        {
            for (int i = 0; i < 10; i++)
            {
                Tile tile = clientBoard[i];
                if(tile.token != null) {
                    Token t = tile.token.GetComponent<Token>();
                    if (t is CreatureToken c) {
                        c.currentHealth += healthMod;
                        c.UpdateStats();
                    }
                }
                
            }
        }
        
        
    }
}
