using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Artifact", fileName = "Card")]
public class Artifact : Card
{

    [Header("Artifact Info")]
    public int defaultHealthAmount;
    public List<attributes> myAttributes;

}
