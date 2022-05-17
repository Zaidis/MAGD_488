using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
public class Menu : MonoBehaviour
{
    public static Menu instance;
    private void Awake() => instance = this;

    public GameObject play;
    [SerializeField] GameObject deckSelect;
    [SerializeField] GameObject settings;
    [SerializeField] GameObject deckEditor;
    [SerializeField] GameObject direct;
    [SerializeField] GameObject credits;
    [SerializeField] GameObject deckSelectPregame;
    [SerializeField] GameObject deckSelectPregameDirect;
    public bool willBeHost = false;
    private void Start() {
        AudioMixer audioMixer = FindObjectOfType<AudioSource>().outputAudioMixerGroup.audioMixer;
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("volume"));
        ButtonSetFalse();
    }    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !play.activeInHierarchy)
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
        deckSelectPregame.SetActive(false);
        deckSelectPregameDirect.SetActive(false);
    }


    public void ButtonPlay()
    {
        MythosClient.instance.OnMatchMake();
        ButtonSetFalse();
        play.SetActive(true);
    }
    public void ButtonPregameDeckSelect() {
        ButtonSetFalse();
        deckSelectPregame.SetActive(true);
        deckEditor.SetActive(true);
    }
    public void ButtonPregameDeckSelectDirect(bool host) {
        ButtonSetFalse();
        deckSelectPregameDirect.SetActive(true);
        willBeHost = host;
        deckEditor.SetActive(true);        
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

    public void ButtonTutorial() {
        SceneManager.LoadScene(5); //tutorial
    }
}
