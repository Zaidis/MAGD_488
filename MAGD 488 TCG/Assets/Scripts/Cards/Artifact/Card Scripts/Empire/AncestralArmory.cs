using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact/AncestralArmory", fileName = "Card")]
public class AncestralArmory : Artifact {

    public Creature zombieToken;

    public override void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent) {
        //base.OnPlay(hostBoard, clientBoard, parent);

        if (parent.hostTile) {

            for (int i = 5; i < 10; i++) {
                if (GameManager.Singleton.hostBoard[i].token != null) {
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
                if (GameManager.Singleton.clientBoard[i].token != null) {
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
        GameManager gm = GameManager.Singleton;
        if (gm.isHost) {
            if (gm.hostCurrentMana < abilityCost)
                return;
            gm.player.UpdateManaServerRpc(gm.hostCurrentMana - abilityCost, gm.clientCurrentMana, gm.hostMaxMana, gm.clientMaxMana);
        } else {
            if (gm.clientCurrentMana < abilityCost)
                return;
            gm.player.UpdateManaServerRpc(gm.hostCurrentMana, gm.clientCurrentMana - abilityCost, gm.hostMaxMana, gm.clientMaxMana);
        }
        gm.myHand.AddCardToHand(zombieToken);
    }
}
