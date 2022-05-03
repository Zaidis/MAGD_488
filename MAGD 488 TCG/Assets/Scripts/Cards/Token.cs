using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public abstract class Token : MonoBehaviour {
    public int currentHealth;  
    //public TextMeshPro Description;    
    //public TextMeshPro Mana;
    [Header("All Token Variables")]
    public TextMeshPro Name;
    public Sprite Art;
    public GameObject cardArtHolder;
    public Image artHolder;
    public GameObject activePartOfTile; //the part that changes material
    public abstract void ApplyCard();

    public abstract void OnPlay();
    public abstract void UpdateStats();
    public void ChangeMaterial(Material m) {
        activePartOfTile.GetComponent<MeshRenderer>().material = m;
    }

}
