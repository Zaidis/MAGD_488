using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Token : MonoBehaviour {
    public int currentHealth;
    public int currentAttack;

    public Card card;

    public TextMeshPro Name;
    public TextMeshPro Description;
    public TextMeshPro Attack;
    public TextMeshPro Mana;
    public Sprite Art;
    public void ApplyCard() {
        if (card.GetType() == typeof(Creature)) {
            Creature c = (Creature)card;
            currentAttack = c.defaultPowerAmount;
            currentHealth = c.defaultHealthAmount;
        } else if (card.GetType() == typeof(Artifact)) {
            Artifact a = (Artifact)card;
            currentHealth = a.defaultHealthAmount;
        }
        Name.text = card.name;
        Description.text = card.description;
        Mana.text = card.manaCost.ToString();
        Art = card.cardArt;
    }
}
