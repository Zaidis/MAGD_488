using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureToken : Token
{
    public int currentAttack;
    public Creature creature;
    public TextMeshPro AttackText;
    public TextMeshPro HealthText;
    public override void ApplyCard() {
        currentAttack = creature.defaultPowerAmount;
        currentHealth = creature.defaultHealthAmount;
        Name.text = creature.cardName;

        AttackText.text = currentAttack.ToString();
        HealthText.text = currentHealth.ToString();

        //Description.text = creature.description;
       // Mana.text = creature.manaCost.ToString();
      //  Art = creature.cardArt;
    }    
}
