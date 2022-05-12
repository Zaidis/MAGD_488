using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact/Bag of Seeds", fileName = "Card")]
public class BagOfSeeds : Artifact
{
    [SerializeField] private int healthMod;
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost) {
        //base.OnAbility(hostBoard, clientBoard, attacker, isHost);
        Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        if (GameManager.Singleton.isHost) {
            if (GameManager.Singleton.hostCurrentMana >= abilityCost) {
                int newMana = GameManager.Singleton.hostCurrentMana - abilityCost;

                //GameManager.Singleton.AffectManaValues(newMana, GameManager.Singleton.clientCurrentMana,
                //    GameManager.Singleton.hostMaxMana, GameManager.Singleton.clientMaxMana);

                p.UpdateManaServerRpc(newMana, GameManager.Singleton.clientCurrentMana,
                    GameManager.Singleton.hostMaxMana, GameManager.Singleton.clientMaxMana);

            }
            else {
                return;
            }
        }
        else {
            if (GameManager.Singleton.clientCurrentMana >= abilityCost) {
                int newMana = GameManager.Singleton.clientCurrentMana - abilityCost;

               // GameManager.Singleton.AffectManaValues(GameManager.Singleton.hostCurrentMana, newMana,
               //     GameManager.Singleton.hostMaxMana, GameManager.Singleton.clientMaxMana);

                p.UpdateManaServerRpc(GameManager.Singleton.hostCurrentMana, newMana,
                    GameManager.Singleton.hostMaxMana, GameManager.Singleton.clientMaxMana);
            }
            else {
                return;
            }
        }

        //Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        if (GameManager.Singleton.isHost) {
            p.UpdateHealthServerRpc(healthMod, 0);
        } else {
            p.UpdateHealthServerRpc(0, healthMod);
        }
        
        //we already know this token has NOT attacked yet

        //token.AttackPlayer(); //animation

        



    }

}
