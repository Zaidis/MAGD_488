using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class SelectCL : MonoBehaviour, IPointerClickHandler
{
    static int limit = 3;
    public Card card;
    public int count;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (count < limit)
            {
                AddToDeck();
                count++;
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
        {

        }
    }

    void AddToDeck()
    {
        if (count == 0)
        {
            GameObject selected = new GameObject("Card " + name, typeof(Image), typeof(SelectDL));
            selected.transform.SetParent(GameObject.Find("DeckList").transform);

            selected.GetComponent<Image>().sprite = card.cardArt;
            selected.GetComponent<SelectDL>().card = this;
        }

        DeckEditor.instance.deckID.Add(card.ID);
    }
}