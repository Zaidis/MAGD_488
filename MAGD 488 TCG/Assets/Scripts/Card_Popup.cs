using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class Card_Popup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProUGUI loreText;
    [SerializeField] private TextMeshProUGUI artistName;
    [SerializeField] private Image cardArt;

    [SerializeField] private GameObject popupObject;

    [SerializeField] private UIAttribute cleave, pierce, lifesteal, thorns;


    public void UpdatePopup(Card card) {

        cleave.gameObject.SetActive(false);
        pierce.gameObject.SetActive(false);
        lifesteal.gameObject.SetActive(false);
        thorns.gameObject.SetActive(false);


        titleText.text = card.cardName;
        descriptionText.text = card.description;
        cardArt.sprite = card.cardArt;
        loreText.text = card.lore;
        artistName.text = "Artist: " + card.artistName;

        if(card is Creature c) {
            if (c.myAttributes.Contains(attributes.cleave)) {
                cleave.gameObject.SetActive(true);
            } 
            if (c.myAttributes.Contains(attributes.pierce)) {
                pierce.gameObject.SetActive(true);
            }
            if (c.myAttributes.Contains(attributes.lifesteal)) {
                lifesteal.gameObject.SetActive(true);
            }
            if (c.myAttributes.Contains(attributes.thorn)) {
                thorns.gameObject.SetActive(true);
            }
        }



        popupObject.SetActive(true);
    }

    public void TurnOffPopup() {
        popupObject.SetActive(false);
    }
}
