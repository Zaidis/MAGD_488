using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtifactToken : Token
{
    public Artifact artifact;
    public override void ApplyCard() {
        currentHealth = artifact.defaultHealthAmount;        
        Name.text = artifact.name;
        //Description.text = artifact.description;
       // Mana.text = artifact.manaCost.ToString();
        Art = artifact.cardArt;
    }
}