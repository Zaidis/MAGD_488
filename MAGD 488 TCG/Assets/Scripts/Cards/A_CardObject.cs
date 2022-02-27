using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class A_CardObject : MonoBehaviour
{
    //for creatures
    public Artifact card;
    [SerializeField] private Image cardArt;
    [SerializeField] private TextMeshProUGUI cardCost;
    [SerializeField] private TextMeshProUGUI cardName;
    [SerializeField] private TextMeshProUGUI cardHealth;
    [SerializeField] private TextMeshProUGUI cardDescription;
    [SerializeField] private GameObject attributeHolder;

    [SerializeField] private GameObject pierceButton;
    [SerializeField] private GameObject rangedButton;
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
        cardHealth.text = card.defaultHealthAmount.ToString();

        //add in attributes
        if (card.myAttributes.Count == 0) {
            return;
        }
        else {
            GameObject temp1 = Instantiate(pierceButton, attributeHolder.transform);
            GameObject temp2 = Instantiate(rangedButton, attributeHolder.transform);
            temp1.transform.parent = attributeHolder.transform;
            temp2.transform.parent = attributeHolder.transform;
        }
    }
}
