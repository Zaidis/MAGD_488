using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class SelectCL : MonoBehaviour, IPointerClickHandler
{
    public Card card;
    public int owned;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Debug.Log("Pressed " + card.cardName + ", you own " + owned + ", will be one less after this message unless already 0.");
            if (owned > 0) // and other criteria met
            {
                owned--;
                AddToDeck();
            }
        }
    }

    void AddToDeck()
    {
        GameObject selected = new GameObject("Card " + name, typeof(Image), typeof(SelectDL));
        selected.transform.parent = GameObject.Find("DeckList").transform;

        selected.GetComponent<Image>().sprite = card.cardArt;
        selected.GetComponent<SelectDL>().card = this;
        DeckEditor.instance.deckID.Add(card.ID);
    }
}