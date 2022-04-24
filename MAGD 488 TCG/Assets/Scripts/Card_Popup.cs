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
    [SerializeField] private Image cardArt;

    [SerializeField] private GameObject popupObject;
    public void UpdatePopup(Card card) {

        titleText.text = card.cardName;
        descriptionText.text = card.description;
        cardArt.sprite = card.cardArt;

        popupObject.SetActive(true);
    }

    public void TurnOffPopup() {
        popupObject.SetActive(false);
    }
}
