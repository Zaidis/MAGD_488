using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hand : MonoBehaviour
{
    private GridLayoutGroup cardGroup;
    private RectTransform myTransform;
    //holds scriptable objects
    public List<Card> myCards = new List<Card>();

    //holds the UI cards on screen
    public List<Image> uiCards = new List<Image>();

    public GameObject emptyCard;
    public float handCenteringAmount;
    private void Start() {
        cardGroup = GetComponent<GridLayoutGroup>();
        myTransform = GetComponent<RectTransform>();
    }

    public void AddCardToHand(Card card) {

        myCards.Add(card);

        GameObject newCard = Instantiate(emptyCard, transform.position, Quaternion.identity);
        newCard.transform.parent = this.transform;

        if(myCards.Count > 1) {
            //myTransform.position = new Vector3(myTransform.rect.xMin - handCenteringAmount, myTransform.rect.yMin, myTransform.rect.xMax);
            myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x - handCenteringAmount, myTransform.anchoredPosition.y);
            //Debug.Log(myTransform.anchoredPosition);
        }

    }

}
