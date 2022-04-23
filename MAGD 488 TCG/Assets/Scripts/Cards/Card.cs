using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card : ScriptableObject
{
    [Header("Basic Card Info")]
    public int ID;
    public cardType type = cardType.creature; //default is creature
    public int manaCost;
    public string cardName;
    public Sprite cardArt;
    public Sprite cardBorder;
    public faction cardFaction = faction.empire;

    [TextArea()]
    public string description;    
}
