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
    }


    public void ButtonPlay()
    {
        MythosClient.instance.OnMatchMake();
        ButtonSetFalse();
        play.SetActive(true);
    }
    public void ButtonDeck()
    {
        ButtonSetFalse();
        deckSelect.SetActive(true);
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
    public void ButtonExit()
    {
        Application.Quit();
    }
}
