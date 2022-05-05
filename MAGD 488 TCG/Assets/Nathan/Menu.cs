using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    private void Awake() => instance = this;

    [SerializeField] GameObject play;
    [SerializeField] GameObject deckSelect;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject deckEditor;
    [SerializeField] GameObject direct;
    [SerializeField] GameObject credits;
    private void Start() => ButtonSetFalse();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ButtonSetFalse();
    }
    public void ButtonSetFalse()
    {
        play.SetActive(false);
        deckSelect.SetActive(false);
        deckEditor.SetActive(false);
        settings.SetActive(false);
        direct.SetActive(false);
        credits.SetActive(false);
    }


    public void ButtonPlay()
    {
        if (TempDeck.instance.usingCustomDeck) {
            TempDeck.instance.AddListToTemporaryDeck(DeckEditor.instance.deckID);
        }


        MythosClient.instance.OnMatchMake();
        ButtonSetFalse();
        play.SetActive(true);
    }
    public void ButtonDirect() {

        if (TempDeck.instance.usingCustomDeck) {
            TempDeck.instance.AddListToTemporaryDeck(DeckEditor.instance.deckID);
        }

        ButtonSetFalse();
        direct.SetActive(true);
    }
    public void ButtonDeck()
    {
        ButtonSetFalse();
        deckSelect.SetActive(true);
        deckEditor.SetActive(true);
    }
    public void ButtonSettings()
    {
        ButtonSetFalse();
        settings.SetActive(true);
    }
    public void OpenEditor()
    {
        ButtonSetFalse();
        deckEditor.SetActive(true);
    }

    public void ButtonCredits() {
        ButtonSetFalse();
        credits.SetActive(true);
    }

    public void ButtonExit()
    {
        Application.Quit();
    }
}
