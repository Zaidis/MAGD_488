using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CreatureToken : Token
{
    public int currentAttack;
    public Creature creature;
    public TextMeshPro Attack;
    public override void ApplyCard() {
        currentAttack = creature.defaultPowerAmount;
        currentHealth = creature.defaultHealthAmount;
        Name.text = creature.name;
        Description.text = creature.description;
        Mana.text = creature.manaCost.ToString();
        Art = creature.cardArt;
    }
}
