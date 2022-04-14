using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler {
    public GameObject token;
    public Transform spawnLocation;
    public void SetToken(GameObject token) {
        this.token = token;
        token.transform.position = spawnLocation.position;
        token.transform.parent = transform;
    }


    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("I clicked on a tile!");
        if (GameManager.Singleton.needsToSelectTile) {
            //we have selected the tile

            if(GameManager.Singleton.selectedCard.type == cardType.creature) {

                Creature c = (Creature)GameManager.Singleton.selectedCard;
                GameObject t = Instantiate(GameManager.Singleton.CreatureTokenPrefab);
                t.GetComponent<CreatureToken>().creature = c;
                t.GetComponent<CreatureToken>().ApplyCard();
                SetToken(t);

                GameManager.Singleton.AffectCurrentMana(c.manaCost * -1);

                Hand.instance.RemoveCardFromHand();

                GameManager.Singleton.ResetSelectedCard();

            }
        }
    }
}
