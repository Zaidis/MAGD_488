using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class O_Ability : MonoBehaviour, IPointerClickHandler {

    public CreatureToken token;


    public void OnPointerClick(PointerEventData eventData) {


        if (token.creature.hasTargetedAbility) {

            /* if (GameManager.Singleton.isHost) {
                 GameManager.Singleton.ActivateTilesWithTokensInBoard(GameManager.Singleton.clientBoard);
             }
             else {
                 GameManager.Singleton.ActivateTilesWithTokensInBoard(GameManager.Singleton.hostBoard);
             } */
            GameManager.Singleton.ActivateAllTilesWithTokens();

            GameManager.Singleton.isUsingAbility = true;
            GameManager.Singleton.selectedCreature = token;
        } else {
            token.UseAbility();
        }
        
        

    }

}
