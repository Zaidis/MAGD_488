using UnityEngine.EventSystems;
using UnityEngine;

public class SelectDL : MonoBehaviour, IPointerClickHandler
{
    public SelectCL card;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            card.owned++;
            DeckEditor.instance.deck.cards.Remove(card.card.ID);
            Destroy(gameObject);
        }
    }
}
