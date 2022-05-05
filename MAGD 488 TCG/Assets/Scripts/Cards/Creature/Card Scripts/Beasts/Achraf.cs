using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Achraf", fileName = "Card")]
public class Achraf : Creature
{
    
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }

    public override void OnTargetedAbility(Tile user, Tile victim, bool isHostSide)
    {
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

        if (victim.token.GetComponent<Token>() is CreatureToken c)
        {
            //TODO: Adam fix meh
            victim.DealtDamage(100);

            if (user.hostTile) {
                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdateHealthServerRpc(2, 0);
            } else {
                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdateHealthServerRpc(0, 2);
            }


        }
    }
}
