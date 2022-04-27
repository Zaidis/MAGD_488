using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact/Default", fileName = "Card")]
public class Artifact : Card
{

    [Header("Artifact Info")]
    public int defaultHealthAmount;
    public List<attributes> myAttributes;


    public virtual void OnPlay(Tile[] hostBoard, Tile[] clientBoard, Tile parent) {
        //do nothing normally
        Debug.LogError("Should not be called...");
    }

}
