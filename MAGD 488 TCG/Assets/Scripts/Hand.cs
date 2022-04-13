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

        myCards.Add(card);

        GameObject newCard = Instantiate(emptyCard, transform.position, Quaternion.identity);
        newCard.transform.parent = this.transform;
        uiCards.Add(newCard);
        //move hand to the left when adding card
        if(myCards.Count > 1) {
            //myTransform.position = new Vector3(myTransform.rect.xMin - handCenteringAmount, myTransform.rect.yMin, myTransform.rect.xMax);
            myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x - handCenteringAmount, myTransform.anchoredPosition.y);
            //Debug.Log(myTransform.anchoredPosition);
            RotateCards();
        }

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
        //rotate to the left
        float temp = maxRotation;
        
        for(int i = 0; i <= right; i++) {
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.rotation = Quaternion.Euler(0, 0, temp);
            temp -= 2;

        }


       /* for (int i = left; i < mid; i++) {
            t = uiCards[i].GetComponent<RectTransform>();
            //uiCards[i].transform.Rotate(new Vector3(uiCards[i].transform.rotation.x, uiCards[i].transform.rotation.y, uiCards[i].transform.rotation.z + rotateCardAmount), Space.Self);
            t.Rotate(new Vector3(0, 0, maxRotation));
            //Quaternion.Euler(new Vector3(t.rotation.x, t.rotation.y, t.rotation.z + rotateCardAmount)); 

        }

        for(int i = mid + 1; i <= right; i++) {
            //uiCards[i].transform.Rotate(new Vector3(uiCards[i].transform.rotation.x, uiCards[i].transform.rotation.y, uiCards[i].transform.rotation.z - rotateCardAmount), Space.Self);
            //uiCards[i].transform.rotation = Quaternion.Euler(uiCards[i].transform.rotation.x, uiCards[i].transform.rotation.y, uiCards[i].transform.rotation.z - rotateCardAmount);
            t = uiCards[i].GetComponent<RectTransform>();
            t.Rotate(new Vector3(0, 0, maxRotation / (i + 1) * -1));
        } */

        uiCards[mid].GetComponent<RectTransform>().Rotate(Vector3.zero);
    }
}
