using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Tile : MonoBehaviour {
    public Token token;

}


public class Token 
{

    public int currentHealth;
    public int currentAttack;
    public int currentManaCost;
    public Token() {
        Token token = new Token();
        if (card.type == cardType.creature) {
            Creature c = (Creature)card;
            token.currentHealth = c.defaultHealthAmount;
            token.currentAttack = c.defaultPowerAmount;
        } else if (card.type == cardType.artifact) {
            Artifact a = (Artifact)card;
            token.currentHealth = a.defaultHealthAmount;
        }
    }
    //public List<attributes> currentAttributes;

}
