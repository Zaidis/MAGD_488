using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ArtifactToken : Token
{
    [Header("Artifact Token Variables")]
    public Artifact artifact;
    public TextMeshPro healthText;
   // public int currentHealth;
    public override void ApplyCard() {
        currentHealth = artifact.defaultHealthAmount;        
        Name.text = artifact.cardName;

        healthText.text = currentHealth.ToString();
        //Description.text = artifact.description;
       // Mana.text = artifact.manaCost.ToString();
       // Art = artifact.cardArt;
    }
}