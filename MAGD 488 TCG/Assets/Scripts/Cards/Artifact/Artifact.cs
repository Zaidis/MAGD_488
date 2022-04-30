using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact/Default", fileName = "Card")]
public class Artifact : Card
{

    [Header("Artifact Info")]
    public int defaultHealthAmount;
    public List<attributes> myAttributes;

    public bool hasAbility;
    public bool hasTargetedAbility;
    public int abilityCost;


    public virtual void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent) {
        //do nothing normally
        Debug.LogError("Should not be called...");
    }

    public virtual void OnAbility(Tile[] hostBoard, Tile[] clientBoard, Tile attacker, bool isHost) {

    }

    public virtual void OnTargetedAbility(Tile user, Tile victim, bool isHostSide) {



    }
}
