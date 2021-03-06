using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Hand : MonoBehaviour
{

    public static Hand instance;

    // private GridLayoutGroup cardGroup;
    private HorizontalLayoutGroup cardGroup;
    private RectTransform myTransform;
    //holds scriptable objects
    public List<Card> myCards = new List<Card>();

    //holds the UI cards on screen
    public List<GameObject> uiCards = new List<GameObject>();

    public GameObject emptyCard;
    public float handCenteringAmount;
    public float rotateCardAmount;

    public Transform cardHover; //for when you hover over a card

    private Vector3 cursorPosition;
    [SerializeField] private Transform cursor_to_hand;
    [SerializeField] private float hand_away_y; //the y value when you are not hovering over the hand 700
    [SerializeField] private float hand_show_y; //the y value when you are hovering over the hand. -490

    public void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
    }

    private void Start() {
        cardGroup = GetComponent<HorizontalLayoutGroup>();
        myTransform = GetComponent<RectTransform>();
    }

   /* private void FixedUpdate() {
        cursorPosition = Input.mousePosition;
        if(cursorPosition.y <= cursor_to_hand.position.y) {
            //Debug.Log("IT WORKS!!!");
            myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x, hand_show_y);
        } else {
            myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x, hand_away_y);
        }
    }*/


    public void AddCardToHand(Card card) {
        
        myCards.Add(card);

        GameObject newCard = Instantiate(emptyCard, transform.position, Quaternion.identity);

        UICard c = newCard.GetComponent<UICard>();
        c.ConjureCard(card);
        newCard.transform.parent = this.transform;
        newCard.transform.localScale = Vector3.one;
        //newCard.GetComponent<Canvas>().sortingOrder = uiCards.Count - 1;
        c.handID = uiCards.Count;

        if(uiCards.Count != 0) {
            int j = uiCards.Count;
            for(int i = 0; i < uiCards.Count; i++) {
                uiCards[i].GetComponent<UICard>().sortingOrder = j;
                uiCards[i].GetComponent<Canvas>().sortingOrder = j;
                j--;
            }
        } else {
            newCard.GetComponent<Canvas>().sortingOrder = 0;
        }

        cardGroup.enabled = true;
        uiCards.Add(newCard);
        //move hand to the left when adding card
        if(myCards.Count > 1) {
            //myTransform.position = new Vector3(myTransform.rect.xMin - handCenteringAmount, myTransform.rect.yMin, myTransform.rect.xMax);
            //myTransform.position = new Vector2(myTransform.position.x - handCenteringAmount, myTransform.position.y);
            cardGroup.padding.left -= (int)handCenteringAmount;
            //Debug.Log(myTransform.anchoredPosition);
            RotateCards();
        }

        GameManager.Singleton.OpponentDrawCard();
        Invoke("DisableGridLayoutGroup", 0.2f);
        
    }

    public void RemoveCardFromHand() {
        Debug.Log("I removed a card");
        cardGroup.enabled = true;

        GameObject c = uiCards[GameManager.Singleton.selectedCardNumber];

        myCards.RemoveAt(GameManager.Singleton.selectedCardNumber);
        uiCards.RemoveAt(GameManager.Singleton.selectedCardNumber);

        Destroy(c);
        // myTransform.position = new Vector2(myTransform.position.x + handCenteringAmount, myTransform.position.y);
        cardGroup.padding.left += (int)handCenteringAmount;
        if (uiCards.Count != 0) {
            if (uiCards.Count > 1) {
                int j = uiCards.Count;
                for (int i = 0; i < uiCards.Count; i++) {
                    uiCards[i].GetComponent<UICard>().handID = i;

                    uiCards[i].GetComponent<UICard>().sortingOrder = j;
                    j--;
                }
                
                
                RotateCards();
            }
            else {
                uiCards[0].GetComponent<UICard>().sortingOrder = 0;
                uiCards[0].GetComponent<UICard>().handID = 0;
            }
        }
        GameManager.Singleton.OpponentUsedCard();
        Invoke("DisableGridLayoutGroup", 0.2f);
    }

    public void UnSelectCards() {
        for(int i = 0; i < uiCards.Count; i++) {
            uiCards[i].GetComponent<UICard>().selectedBorder.gameObject.SetActive(false);
        }

        GameManager.Singleton.needsToSelectTile = false;
        GameManager.Singleton.selectedCard = null;
        GameManager.Singleton.selectedCardNumber = -5;
        GameManager.Singleton.ChangeAllTileMaterials();
    }
    private void RotateCards() {
        int left = 0, right = uiCards.Count - 1;
        int mid = (left + right) / 2;

        float maxRotation = rotateCardAmount * mid + 1;
        
        if(uiCards.Count % 2 != 0) {
            maxRotation -= rotateCardAmount; //if the amount is odd, then the middle should be taken out of the equation. 
        }

        //Debug.Log(maxRotation);
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

    public void PositionCards() {
        
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
        //Debug.Log("+++" + temp);
        for (int i = 0; i <= mid; i++) {
            c = uiCards[i].GetComponent<UICard>();
            c.defaultPosition_Y = temp;
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.position = new Vector2(t.position.x, temp);
            temp += 2;

        }
        temp = (50 + maxPositionChange) * -1f;
        for (int i = right; i >= mid; i--) {
            c = uiCards[i].GetComponent<UICard>();
            c.defaultPosition_Y = temp;
            t = uiCards[i].GetComponent<RectTransform>();

            //t.Rotate(new Vector3(0, 0, temp));
            t.position = new Vector2(t.position.x, temp);
            temp += 2;

        }
    }
    private void DisableGridLayoutGroup() {
       // cardGroup.enabled = false; //so we can move cards

        if(uiCards.Count > 1)
            PositionCards();
    }
}
