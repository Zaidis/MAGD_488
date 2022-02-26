using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "New Creature", fileName = "Card")]
public class Creature : Card
{
   

    [Header("Creature Info")]
    public int defaultHealthAmount;
    public int defaultPowerAmount; //attack damage
    public List<string> attributes;
    

}
