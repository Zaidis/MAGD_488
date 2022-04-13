using UnityEngine.EventSystems;
using UnityEngine;

public class SelectDL : MonoBehaviour, IPointerClickHandler
{
    public SelectCL card;

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
        {

        }
    }
}
