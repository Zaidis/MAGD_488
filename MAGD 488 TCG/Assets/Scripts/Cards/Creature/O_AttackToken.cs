using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class O_AttackToken : MonoBehaviour, IPointerClickHandler
{

    public CreatureToken token;

    public void OnPointerClick(PointerEventData eventData) {


        //we already know this token has NOT attacked yet
        if (GameManager.Singleton.isHost) {
            GameManager.Singleton.ResetAllTiles(GameManager.Singleton.clientBoard);
            if (token.creature.isMelee) {
                GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.clientBoard, true, token.transform.parent.GetComponent<Tile>().GetTileID());
            }
            else {
                GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.clientBoard, false, token.transform.parent.GetComponent<Tile>().GetTileID());
            }
        } else {
            if (token.creature.isMelee) {
                GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.hostBoard, true, token.transform.parent.GetComponent<Tile>().GetTileID());
            }
            else {
                GameManager.Singleton.ChangeTilesMaterial(GameManager.Singleton.hostBoard, false, token.transform.parent.GetComponent<Tile>().GetTileID());
            }
        }
        

        GameManager.Singleton.isAttecking = true;
        GameManager.Singleton.selectedCreature = token;
        
    }

}
