using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "New Creature/MistressOfTheFrozenSwamp")]
public class MistressOfTheFrozenSwamp : Creature
{
    [SerializeField] int attackMod, healthMod, abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker,  bool isHost)
    {
        //buff/big buff if hunger creature - 3 mana cost
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
        Token t = victim.token.GetComponent<Token>();
        if(t is CreatureToken c)
        {
            c.currentAttack += attackMod;
            c.currentHealth += healthMod;
            c.UpdateStats();
        }
    }
}
