using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class O_Ability : MonoBehaviour, IPointerClickHandler {

    public Token token;


    public void OnPointerClick(PointerEventData eventData) {

        if(token.GetComponent<Token>() is CreatureToken c) {
            if (c.creature.hasTargetedAbility) {

                /* if (GameManager.Singleton.isHost) {
                     GameManager.Singleton.ActivateTilesWithTokensInBoard(GameManager.Singleton.clientBoard);
                 }
                 else {
                     GameManager.Singleton.ActivateTilesWithTokensInBoard(GameManager.Singleton.hostBoard);
                 } */
                GameManager.Singleton.ActivateAllTilesWithTokens();

                GameManager.Singleton.isUsingAbility = true;
                GameManager.Singleton.selectedCreature = c;
            }
            else {
                c.UseAbility();
            }
        } else if(token.GetComponent<Token>() is ArtifactToken a) {

            if (a.artifact.hasTargetedAbility) {
                GameManager.Singleton.ActivateAllTilesWithTokens();
                GameManager.Singleton.isUsingAbility = true;
                
            } else {
                a.UseAbility();
            }

        }
        
        
        

    }

}
