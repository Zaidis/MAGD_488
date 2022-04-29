using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/Shinigami")]
public class Shinigami : Creature
{
    [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //Lifesteal 3 Mana, Heals player equal to attack

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

        if (attacker.hostTile)
        {
            Token t = attacker.token.GetComponent<Token>();
            if(t is CreatureToken c)
            {
                //GameManager.Singleton.AffectHealthValues(c.currentAttack, 0);
                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdateHealthServerRpc(c.currentAttack, 0);
            }

        }
        else
        {
            Token t = attacker.token.GetComponent<Token>();
            if (t is CreatureToken c)
            {
               // GameManager.Singleton.AffectHealthValues(0, c.currentAttack);
                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdateHealthServerRpc(0, c.currentAttack);
            }
        }
    }
}