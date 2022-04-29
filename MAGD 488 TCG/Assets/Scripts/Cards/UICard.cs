using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Card myCard;
    public float defaultRotation_Z;
    public float defaultPosition_Y;
    public int sortingOrder; //UI canvas sorting order
    public int handID; //place in hand array

    private RectTransform myTransform;
    [Header("UI Information")]
    [SerializeField] private Image cardArt;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardAttack;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private TextMeshProUGUI manaCost;
    [SerializeField] private Image cardBorderArt;
    private void Start() {
        myTransform = GetComponent<RectTransform>();
    }

    public void ConjureCard(Card card) {
        myCard = card;
        cardName.text = card.cardName;
        cardDescription.text = card.description;

        cardArt.sprite = card.cardArt;
        cardBorderArt.sprite = card.cardBorder;
        manaCost.text = card.manaCost.ToString();

        if(card.type == cardType.creature) {
            Creature c = (Creature)card;
            cardAttack.text = c.defaultPowerAmount.ToString();
            cardHealth.text = c.defaultHealthAmount.ToString();
        } else if (card.type == cardType.artifact) {
            Artifact a = (Artifact)card;
            cardHealth.text = a.defaultHealthAmount.ToString();
        }
    }

    public void OnPointerClick(PointerEventData eventData) {

        //if you LEFT CLICK
        if(eventData.button == PointerEventData.InputButton.Left) {

            if ((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {

                if (GameManager.Singleton.isHost) {
                    if (GameManager.Singleton.hostCurrentMana >= myCard.manaCost) {
                        //we can use the card. now we need to select a place to put it
                        if (myCard.type == cardType.spell) {
                            //we do not need a place to put the spell. it just works.
                        }
                        else {
                            //we need to select a place to put the card. 
                            GameManager.Singleton.needsToSelectTile = true;
                            GameManager.Singleton.selectedCard = myCard;
                            GameManager.Singleton.selectedCardNumber = handID;

                        }
                    }
                } else {
                    if (GameManager.Singleton.clientCurrentMana >= myCard.manaCost) {
                        //we can use the card. now we need to select a place to put it
                        if (myCard.type == cardType.spell) {
                            //we do not need a place to put the spell. it just works.
                        }
                        else {
                            //we need to select a place to put the card. 
                            GameManager.Singleton.needsToSelectTile = true;
                            GameManager.Singleton.selectedCard = myCard;
                            GameManager.Singleton.selectedCardNumber = handID;

                        }
                    }
                }
                
            }
        } else if (eventData.button == PointerEventData.InputButton.Right) {
            Debug.Log("Updating popup");
            //will bring up a pop up to see the card more closely and also read lore. 
            GameManager.Singleton.panelPopup.UpdatePopup(myCard);
            
        }

    }


    //When you hover over card. REQUIRES GRAPHIC RAYCASTER 
    public void OnPointerEnter(PointerEventData eventData) {

        Debug.Log("Hovering over card");
        GetComponent<Canvas>().sortingOrder = 20;

       // myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x, GameManager.Singleton.myHand.cardHover.GetComponent<RectTransform>().position.y); //fix later
        myTransform.position = new Vector2(myTransform.position.x, GameManager.Singleton.myHand.cardHover.GetComponent<RectTransform>().position.y);
        myTransform.rotation = Quaternion.Euler(0, 0, 0);


    }

    //when you stop hovering over card
    public void OnPointerExit(PointerEventData eventData) {

        GetComponent<Canvas>().sortingOrder = sortingOrder;
        myTransform.rotation = Quaternion.Euler(0, 0, defaultRotation_Z);

        myTransform.position = new Vector2(myTransform.position.x, defaultPosition_Y);
       // myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x, defaultPosition_Y);
    }
}
