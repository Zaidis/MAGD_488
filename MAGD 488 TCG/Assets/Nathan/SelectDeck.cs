using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using Unity.Netcode;
using UnityEngine.SceneManagement;

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
    public void ButtonSelectDirect() {        
        MythosClient.OnDeckContentLoaded += SetDeckDirect;
        MythosClient.instance.OnRetrieveDeckContent(name);
        DeckEditor.instance.SetDeckName(name);        
    }
    private void SetDeck(List<int> deck) {
        MythosClient.OnDeckContentLoaded -= SetDeck;
        TempDeck.instance.AddListToTemporaryDeck(DeckEditor.instance.deckID);
        TempDeck.instance.usingCustomDeck = true;
        Menu.instance.ButtonPlay();
    }
    private void SetDeckDirect(List<int> deck) {
        MythosClient.OnDeckContentLoaded -= SetDeckDirect;
        TempDeck.instance.AddListToTemporaryDeck(DeckEditor.instance.deckID);
        TempDeck.instance.usingCustomDeck = true;        
        if (!Menu.instance.willBeHost)
            NetworkManager.Singleton.StartClient();
        SceneManager.LoadScene("Game");
    }
}
