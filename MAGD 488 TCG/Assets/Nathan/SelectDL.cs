using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
public class SelectDL : MonoBehaviour, IPointerClickHandler
{
    public SelectCL card;
    [SerializeField] TextMeshProUGUI info;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            card.count--;
            UpdateInfo();
            DeckEditor.instance.deckID.Remove(card.card.ID);
            if (card.count == 0)
            {
                DeckEditor.instance.SetDeckListButtonAmount(-1);
                Destroy(gameObject);
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
            DeckEditor.instance.DisplayCard(card.card);
    }

    public void UpdateInfo() {
        if (card != null) {
            info.text = card.count + " | " + card.card.cardName;
        } 
    }
}
