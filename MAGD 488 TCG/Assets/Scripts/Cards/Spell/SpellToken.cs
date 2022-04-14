using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellToken : Token 
{
    public Spell spell;
    public override void ApplyCard() {
        Name.text = spell.name;
        //Description.text = spell.description;
       // Mana.text = spell.manaCost.ToString();
        Art = spell.cardArt;
    }
}