using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class O_AttackPlayer : MonoBehaviour, IPointerClickHandler
{

    public CreatureToken token;

    public void OnPointerClick(PointerEventData eventData) {


        //we already know this token has NOT attacked yet

        if (GameManager.Singleton.isHost) {
            GameManager.Singleton.clientHealth -= token.currentAttack;
        } else {
            GameManager.Singleton.hostHealth -= token.currentAttack;
        }

        token.hasAttacked = true;
        GameManager.Singleton.CreatureOptionButtons(token, GameManager.Singleton.isHost);
    }
}
