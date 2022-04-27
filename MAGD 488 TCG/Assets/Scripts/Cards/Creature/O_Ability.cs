using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class O_Ability : MonoBehaviour, IPointerClickHandler {

    public CreatureToken token;


    public void OnPointerClick(PointerEventData eventData) {


        
        token.UseAbility();
        

    }

}
