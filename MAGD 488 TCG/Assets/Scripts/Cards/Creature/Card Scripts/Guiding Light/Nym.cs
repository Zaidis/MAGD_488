using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Nym", fileName = "Card")]
public class Nym : Creature
{
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //All friendly ranged creatures +1/+1

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

        if (attacker.hostTile) {
            //buff all host ranged creatures

            for(int i = 0; i < 5; i++) {
                Token t = GameManager.Singleton.hostBoard[i].token.GetComponent<Token>();

                t.currentHealth += 1;
                if(t is CreatureToken c) {
                    c.currentAttack += 1;
                }

                t.UpdateStats();

            }
        } else {
            for (int i = 0; i < 5; i++) {
                Token t = GameManager.Singleton.clientBoard[i].token.GetComponent<Token>();

                t.currentHealth += 1;
                if (t is CreatureToken c) {
                    c.currentAttack += 1;
                }

                t.UpdateStats();

            }
        }

    }

    
}
