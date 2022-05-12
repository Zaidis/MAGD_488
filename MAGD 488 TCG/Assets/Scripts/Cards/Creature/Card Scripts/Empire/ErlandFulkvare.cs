using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/ErlandFulkvare", fileName = "Card")]
public class ErlandFulkvare : Creature
{
    [SerializeField] int healthMod;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
        
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //All cards,0/+2 and Taunt (4 Mana Cost)
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

        //Token t = attacker.token.GetComponent<Token>();
        if (isHost) {
            for (int i = 0; i < 10; i++) {
                Tile tile = hostBoard[i];
                if(tile.token != null) {
                    Token t = tile.token.GetComponent<Token>();
                    if (t is CreatureToken c) {
                        c.currentHealth += healthMod;
                        c.UpdateStats();
                    }
                }
                
            }
        } else {
            for (int i = 0; i < 10; i++) {
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
