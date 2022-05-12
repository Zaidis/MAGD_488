using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/Shinigami")]
public class Shinigami : Creature {
    // [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked) {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost) {
        //Lifesteal 3 Mana, Heals player equal to attack
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
            Token t = attacker.token.GetComponent<Token>();
            if (t is CreatureToken c) {
                //GameManager.Singleton.AffectHealthValues(c.currentAttack, 0);
                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdateHealthServerRpc(c.currentAttack, 0);
            }

        } else {
            Token t = attacker.token.GetComponent<Token>();
            if (t is CreatureToken c) {
                // GameManager.Singleton.AffectHealthValues(0, c.currentAttack);
                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdateHealthServerRpc(0, c.currentAttack);
            }
        }
    }
}