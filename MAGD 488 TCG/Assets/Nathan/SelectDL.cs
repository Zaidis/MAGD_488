using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

public class SelectDL : MonoBehaviour, IPointerClickHandler
{
    public SelectCL card;
    [SerializeField] TextMeshProUGUI info;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            card.count--;

            if (card.count == 0)
            {
                DeckEditor.instance.deckID.Remove(card.card.ID);
                Destroy(gameObject);
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
            DeckEditor.instance.DisplayCard(card.card);
    }
    private void Update()
    {
        if (card != null && info != null)
            info.text = card.count + " | " + card.card.cardName;
    }
}
