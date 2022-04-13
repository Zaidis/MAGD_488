using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    public static Menu instance;
    private void Awake() => instance = this;

    [SerializeField] GameObject play;
    [SerializeField] GameObject deck;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject Editor;

    private void Start() => ButtonSetFalse();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            ButtonSetFalse();
    }
    public void ButtonSetFalse()
    {
        play.SetActive(false);
        deck.SetActive(false);
        Editor.SetActive(false);
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
        deck.SetActive(true);
    }
    public void ButtonSettings()
    {
        ButtonSetFalse();
        settings.SetActive(true);
    }
    public void OpenEditor()
    {
        ButtonSetFalse();
        Editor.SetActive(true);
    }
    public void ButtonExit()
    {
        Application.Quit();
    }
}
