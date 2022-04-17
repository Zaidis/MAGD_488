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
            DeckEditor.instance.DisplayCard(card);
    }

    void AddToDeck()
    {
        if (count == 0)
        {
            GameObject selected = Instantiate(DeckEditor.instance.PrefabDL);
            selected.transform.SetParent(GameObject.Find("DeckList").transform);
            selected.GetComponent<SelectDL>().card = this;
        }

        DeckEditor.instance.deckID.Add(card.ID);
    }
}