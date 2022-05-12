using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Nym", fileName = "Card")]
public class Nym : Creature {
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked) {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost) {
        //All friendly ranged creatures +1/+1
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

        if (attacker.hostTile) {
            //buff all host ranged creatures

            for (int i = 0; i < 5; i++) {
                Token t = GameManager.Singleton.hostBoard[i].token.GetComponent<Token>();

                t.currentHealth += 1;
                if (t is CreatureToken c) {
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
