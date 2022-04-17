using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public abstract class Token : MonoBehaviour {
    public int currentHealth;  
    //public TextMeshPro Description;    
    //public TextMeshPro Mana;
    [Header("All Token Variables")]
    public TextMeshPro Name;
    public Sprite Art;


    public abstract void ApplyCard();
}
