using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/Achraf", fileName = "Card")]
public class Achraf : Creature
{
    [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {

    }

    public override void OnTargetedAbility(Tile user, Tile victim, bool isHostSide)
    {
        if (GameManager.Singleton.currentMana >= abilityCost)
        {
            GameManager.Singleton.AffectCurrentMana(-abilityCost);
        }
        else
        {
            return;
        }

        if (victim.token.GetComponent<Token>() is CreatureToken c)
        {
            //TODO: Adam fix meh
        }
    }
}
