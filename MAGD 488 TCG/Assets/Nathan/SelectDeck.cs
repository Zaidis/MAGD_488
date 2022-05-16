using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;

public class SelectDeck : MonoBehaviour, IPointerClickHandler
{
    public new string name;
    public TextMeshProUGUI Text_name;
    public void OnPointerClick(PointerEventData eventData)
    {
        Menu.instance.OpenEditor();
        DeckEditor.instance.SetDeckName(name);
    }
    private void Start()
    {
        Text_name.text = name;        
    }
    public void ButtonEdit()
    {        

        Menu.instance.OpenEditor();
        MythosClient.instance.OnRetrieveDeckContent(name);
        DeckEditor.instance.SetDeckName(name);

        //TempDeck.instance.AddListToTemporaryDeck(DeckEditor.instance.deckID);
        //TempDeck.instance.usingCustomDeck = true;
    }
    public void ButtonDelete()
    {
        MythosClient.instance.OnDeleteDeck(name);
        Destroy(transform.parent.gameObject);
    }
    public void ButtonSelect() {
        MythosClient.OnDeckContentLoaded += SetDeck;
        Menu.instance.play.SetActive(true);
        MythosClient.instance.OnRetrieveDeckContent(name);
        DeckEditor.instance.SetDeckName(name);        
    }
    private void SetDeck(List<int> deck) {        
        TempDeck.instance.AddListToTemporaryDeck(DeckEditor.instance.deckID);
        TempDeck.instance.usingCustomDeck = true;
        Menu.instance.ButtonPlay();
    }
}
