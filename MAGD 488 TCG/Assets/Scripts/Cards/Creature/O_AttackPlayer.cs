using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class O_AttackPlayer : MonoBehaviour, IPointerClickHandler
{

    public CreatureToken token;

    public void OnPointerClick(PointerEventData eventData) {

        Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        //we already know this token has NOT attacked yet

        if (GameManager.Singleton.isHost) {
            //GameManager.Singleton.AffectClientCurrentHealth(token.currentAttack * -1);
            p.UpdateHealth(true, 0, token.currentAttack * -1);
        } else {
            //GameManager.Singleton.AffectHostCurrentHealth(token.currentAttack * -1);
            p.UpdateHealth(false, token.currentAttack * -1, 0);
        }

        token.hasAttacked = true;
        GameManager.Singleton.CreatureOptionButtons(token, GameManager.Singleton.isHost);
    }
}
