using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "New Creature/ErlandFulkvare", fileName = "Card")]
public class ErlandFulkvare : Creature
{
    [SerializeField] int abilityCost, healthMod;
    public override void OnAttack(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost, Tile attacked)
    {
        base.OnAttack(hostBoard, clientBoard, attacker, isHost, attacked);
        
    }
    public override void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost)
    {
        //All cards,0/+2 and Taunt (4 Mana Cost)
        if (GameManager.Singleton.currentMana >= abilityCost)
        {
            GameManager.Singleton.AffectCurrentMana(-abilityCost);
        }
        else
        {
            return;
        }
        //Token t = attacker.token.GetComponent<Token>();
        if (isHost)
        {
            for (int i = 0; i < 10; i++)
            {
                Tile tile = hostBoard[i];
                Token t = tile.token.GetComponent<Token>();
                if(t is CreatureToken c)
                {
                    c.currentHealth += healthMod;
                    c.UpdateStats();
                }
            }
        } else
        {
            for (int i = 0; i < 10; i++)
            {
                Tile tile = clientBoard[i];
                Token t = tile.token.GetComponent<Token>();
                if (t is CreatureToken c)
                {
                    c.currentHealth += healthMod;
                    c.UpdateStats();
                }
            }
        }
        
        
    }
}
