using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class S_CardObject : MonoBehaviour
{
    //for creatures
    public Spell card;
    [SerializeField] private Image cardArt;
    [SerializeField] private TextMeshProUGUI cardCost;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardDescription;
    private void Start() {
        UpdateCardVariables();
    }

    /// <summary>
    /// Updates all card variables.
    /// </summary>
    private void UpdateCardVariables() {
        cardCost.text = card.manaCost.ToString();
        cardName.text = card.cardName.ToString();
        cardArt.sprite = card.cardArt;
        cardDescription.text = card.description.ToString();

        //add in attributes
    }
}
