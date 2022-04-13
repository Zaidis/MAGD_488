using UnityEngine.EventSystems;
using UnityEngine;

public class SelectDL : MonoBehaviour, IPointerClickHandler
{
    public SelectCL card;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
<<<<<<< Updated upstream
            card.owned++;
            DeckEditor.instance.deck.cards.Remove(card.card.ID);
            Destroy(gameObject);
=======
            card.count--;
            DeckEditor.instance.deckID.Remove(card.card.ID);

            if (card.count == 0)
                Destroy(gameObject);
>>>>>>> Stashed changes
        }
    }
}
