using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/LordOfTheGnashingTeeth", fileName = "Card")]
public class LordOfTheGnashingTeeth : Creature
{
    [SerializeField] int abilityCost;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);

    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //self-buff add thorns for the next 2 turns + -2/0 - 2 mana cost
        //TODO: Rn we just git thorns boi. We ned thyme tuh orc.
        if (GameManager.Singleton.currentMana >= abilityCost)
        {
            Token t = attacker.token.GetComponent<Token>();
            if(t is CreatureToken c)
            {
                c.myAttributes.Add(attributes.thorn);
            }
            GameManager.Singleton.AffectCurrentMana(-abilityCost);
        }
    }
}
