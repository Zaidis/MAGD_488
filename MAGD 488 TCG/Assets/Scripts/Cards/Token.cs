using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Token : MonoBehaviour {
    public int currentHealth;  
    public TextMeshPro Name;
    public TextMeshPro Description;    
    public TextMeshPro Mana;
    public Sprite Art;

    public abstract void ApplyCard();
}
