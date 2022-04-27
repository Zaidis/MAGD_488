using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class Hover_Popup : MonoBehaviour
{
    [SerializeField] private Image cardArt;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardAttack;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private TextMeshProUGUI manaCost;
    [SerializeField] private Image cardBorderArt;
    public void UpdateHoverPopup(Card card) {

        cardName.text = card.cardName;
        cardDescription.text = card.description;
        cardArt.sprite = card.cardArt;
        cardBorderArt.sprite = card.cardBorder;
        manaCost.text = card.manaCost.ToString();

        if (card is Creature c) {

            cardAttack.text = c.defaultPowerAmount.ToString();
            cardHealth.text = c.defaultHealthAmount.ToString();

        } else if (card is Artifact a) {

            cardHealth.text = a.defaultHealthAmount.ToString();

        }

    }

}
