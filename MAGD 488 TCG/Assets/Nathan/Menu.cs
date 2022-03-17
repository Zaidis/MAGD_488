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

    private void Start() => SetFalse();
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            SetFalse();
    }
    void SetFalse()
    {
        play.SetActive(false);
        deck.SetActive(false);
        Editor.SetActive(false);
        settings.SetActive(false);
    }


    public void ButtonPlay()
    {
        SetFalse();
        play.SetActive(true);
    }
    public void ButtonDeck()
    {
        SetFalse();
        deck.SetActive(true);
    }
    public void ButtonSettings()
    {
        SetFalse();
        settings.SetActive(true);
    }
    public void OpenEditor()
    {
        SetFalse();
        Editor.SetActive(true);
    }
    public void ButtonExit()
    {
        Application.Quit();
    }
}
