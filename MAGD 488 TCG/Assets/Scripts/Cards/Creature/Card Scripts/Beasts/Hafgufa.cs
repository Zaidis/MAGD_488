using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Hafgufa", fileName = "Card")]
public class Hafgufa : Creature
{

    public Creature newToken;

    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }

    public override void OnTargetedAbility(Tile user, Tile victim, bool isHostSide) {
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
        if (victim.token.GetComponent<Token>() is CreatureToken c) {
            //TODO: Adam fix meh
            victim.DealtDamage(100); //sacrifice
            gm.myHand.AddCardToHand(newToken);
        }
    }
}
