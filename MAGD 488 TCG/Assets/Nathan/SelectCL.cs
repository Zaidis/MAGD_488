using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SelectCL : MonoBehaviour, IPointerClickHandler
{
    static int limit = 3;
    public Card card;
    public int count;

    private SelectDL selectDL;
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (count < limit)
            {
                AddToDeck();                
            }
        }
        if (eventData.button == PointerEventData.InputButton.Right)
            DeckEditor.instance.DisplayCard(card);
    }
    public void OnLoadDeckInteract() {
        if(count < limit)
            AddToDeck();
    }

    void AddToDeck()
    {
        if(DeckEditor.instance.deckID.Count < 40) {
            if (count == 0) {
                GameObject selected = Instantiate(DeckEditor.instance.PrefabDL);
                selected.transform.SetParent(GameObject.Find("DeckList").transform);
                selected.GetComponent<SelectDL>().card = this;
                count++;
                selected.GetComponent<SelectDL>().UpdateInfo();
                selectDL = selected.GetComponent<SelectDL>();
                DeckEditor.instance.SetDeckListButtonAmount(1);
            }
            else {
                count++;
                selectDL.UpdateInfo();
            }

            DeckEditor.instance.deckID.Add(card.ID);
        }
        
    }
}