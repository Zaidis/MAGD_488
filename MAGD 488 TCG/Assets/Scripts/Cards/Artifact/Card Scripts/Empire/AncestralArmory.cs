using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact/AncestralArmory", fileName = "Card")]
public class AncestralArmory : Artifact
{

    public Creature zombieToken;
    
    public override void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent) {
        //base.OnPlay(hostBoard, clientBoard, parent);

        if (parent.hostTile) {

            for (int i = 5; i < 10; i++) {
                if(GameManager.Singleton.hostBoard[i].token != null) {
                    Token t = GameManager.Singleton.hostBoard[i].token.GetComponent<Token>();

                    t.currentHealth += 1;
                    if (t is CreatureToken c) {
                        c.currentAttack += 1;
                        c.currentHealth += 1;
                    }

                    t.UpdateStats();
                }
                

            }

        } else {
            for (int i = 5; i < 10; i++) {
                if(GameManager.Singleton.clientBoard[i].token != null) {
                    Token t = GameManager.Singleton.clientBoard[i].token.GetComponent<Token>();

                    t.currentHealth += 1;
                    if (t is CreatureToken c) {
                        c.currentAttack += 1;
                        c.currentHealth += 1;
                    }

                    t.UpdateStats();
                }
                

            }
        }


    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost) {
        //base.OnAbility(hostBoard, clientBoard, attacker, isHost);

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

        GameManager.Singleton.myHand.AddCardToHand(zombieToken);

    }


}
