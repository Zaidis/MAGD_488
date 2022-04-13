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
    public List<GameObject> uiCards = new List<GameObject>();

    public GameObject emptyCard;
    public float handCenteringAmount;
    public float rotateCardAmount;
    private void Start() {
        cardGroup = GetComponent<GridLayoutGroup>();
        myTransform = GetComponent<RectTransform>();
    }

    public void AddCardToHand(Card card) {
        cardGroup.enabled = true;
        myCards.Add(card);

        GameObject newCard = Instantiate(emptyCard, transform.position, Quaternion.identity);

        UICard c = newCard.GetComponent<UICard>();
        newCard.transform.parent = this.transform;
        newCard.GetComponent<Canvas>().sortingOrder = uiCards.Count;
        c.sortingOrder = uiCards.Count;

        uiCards.Add(newCard);
        //move hand to the left when adding card
        if(myCards.Count > 1) {
            //myTransform.position = new Vector3(myTransform.rect.xMin - handCenteringAmount, myTransform.rect.yMin, myTransform.rect.xMax);
            myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x - handCenteringAmount, myTransform.anchoredPosition.y);
            //Debug.Log(myTransform.anchoredPosition);
            RotateCards();
        }

        Invoke("DisableGridLayoutGroup", 0.2f);
        
    }

    private void RotateCards() {
        int left = 0, right = uiCards.Count - 1;
        int mid = (left + right) / 2;

        float maxRotation = rotateCardAmount * mid + 1;
        
        if(uiCards.Count % 2 != 0) {
            maxRotation -= rotateCardAmount; //if the amount is odd, then the middle should be taken out of the equation. 
        }

        Debug.Log(maxRotation);
        RectTransform t;
        UICard c;
        //rotate to the left
        float temp = maxRotation;
        
        for(int i = 0; i <= right; i++) {
            t = uiCards[i].GetComponent<RectTransform>();
            c = uiCards[i].GetComponent<UICard>();
            //t.Rotate(new Vector3(0, 0, temp));
            t.rotation = Quaternion.Euler(0, 0, temp);
            c.defaultRotation_Z = temp;
            temp -= 2;

        }
        uiCards[mid].GetComponent<RectTransform>().Rotate(Vector3.zero);
    }

    private void PositionCards() {
        int left = 0, right = uiCards.Count - 1;
        int mid = (left + right) / 2;

        float maxPositionChange = rotateCardAmount * mid + 1;

        if (uiCards.Count % 2 != 0) {
            maxPositionChange -= rotateCardAmount; //if the amount is odd, then the middle should be taken out of the equation. 
        }

        
        RectTransform t;
        UICard c;
        //rotate to the left
        float temp = (50 + maxPositionChange) * -1f; //everything begins at 50
        Debug.Log("+++" + temp);
        for (int i = 0; i <= mid; i++) {
            c = uiCards[i].GetComponent<UICard>();
            c.defaultPosition_Y = temp;
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.anchoredPosition = new Vector2(t.anchoredPosition.x, temp);
            temp += 2;

        }
        temp = (50 + maxPositionChange) * -1f;
        for (int i = right; i >= mid; i--) {
            c = uiCards[i].GetComponent<UICard>();
            c.defaultPosition_Y = temp;
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.anchoredPosition = new Vector2(t.anchoredPosition.x, temp);
            temp += 2;

        }
    }
    private void DisableGridLayoutGroup() {
        cardGroup.enabled = false; //so we can move cards
        PositionCards();
    }
}
