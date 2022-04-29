using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class OpponentHand : MonoBehaviour
{
    public List<GameObject> uiCards = new List<GameObject>();
    public GameObject emptyCard;
    public float handCenteringAmount;
    public float rotateCardAmount;

    private RectTransform myTransform;
    private HorizontalLayoutGroup cardGroup;
    private void Start() {
        myTransform = GetComponent<RectTransform>();
        cardGroup = GetComponent<HorizontalLayoutGroup>();
    }
    public void AddCardToHand() {

        GameObject newCard = Instantiate(emptyCard, transform.position, Quaternion.identity);
        newCard.transform.parent = this.transform;
        newCard.transform.localScale = Vector3.one;
        if (uiCards.Count != 0) {
            int j = uiCards.Count;
            for (int i = 0; i < uiCards.Count; i++) {
                uiCards[i].GetComponent<Canvas>().sortingOrder = j;
                j--;
            }
        }
        else {
            newCard.GetComponent<Canvas>().sortingOrder = 0;
        }
        uiCards.Add(newCard);

        if (uiCards.Count > 1) {
            //myTransform.position = new Vector3(myTransform.rect.xMin - handCenteringAmount, myTransform.rect.yMin, myTransform.rect.xMax);
            //myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x - handCenteringAmount, myTransform.anchoredPosition.y);
            //Debug.Log(myTransform.anchoredPosition);
            cardGroup.padding.left -= (int)handCenteringAmount;
           // RotateCards();
        }
    }


    public void RemoveCardFromHand() {


        GameObject c = uiCards[GameManager.Singleton.selectedCardNumber];

        uiCards.RemoveAt(GameManager.Singleton.selectedCardNumber);

        Destroy(c);
        cardGroup.padding.left += (int)handCenteringAmount;
        //myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x + handCenteringAmount, myTransform.anchoredPosition.y);
    }

    private void RotateCards() {
        int left = 0, right = uiCards.Count - 1;
        int mid = (left + right) / 2;

        float maxRotation = rotateCardAmount * mid + 1;

        if (uiCards.Count % 2 != 0) {
            maxRotation -= rotateCardAmount; //if the amount is odd, then the middle should be taken out of the equation. 
        }

        //Debug.Log(maxRotation);
        RectTransform t;
        
        //rotate to the left
        float temp = maxRotation;

        for (int i = 0; i <= right; i++) {
            t = uiCards[i].GetComponent<RectTransform>();
            //t.Rotate(new Vector3(0, 0, temp));
            t.rotation = Quaternion.Euler(0, 0, temp);
            temp -= 2;

        }
        uiCards[mid].GetComponent<RectTransform>().Rotate(Vector3.zero);
    }
    public void PositionCards() {

        int left = 0, right = uiCards.Count - 1;
        int mid = (left + right) / 2;

        float maxPositionChange = rotateCardAmount * mid + 1;

        if (uiCards.Count % 2 != 0) {
            maxPositionChange -= rotateCardAmount; //if the amount is odd, then the middle should be taken out of the equation. 
        }


        RectTransform t;
        

        //rotate to the left
        float temp = (50 + maxPositionChange) * -1f; //everything begins at 50
        //Debug.Log("+++" + temp);
        for (int i = 0; i <= mid; i++) {
            
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.anchoredPosition = new Vector2(t.anchoredPosition.x, temp);
            temp += 2;

        }
        temp = (50 + maxPositionChange) * -1f;
        for (int i = right; i >= mid; i--) {
           
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.anchoredPosition = new Vector2(t.anchoredPosition.x, temp);
            temp += 2;

        }
    }
}
