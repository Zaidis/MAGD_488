using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class GameMenu : MonoBehaviour
{

    public bool turnedOn;

    public GameObject settingsMenu;

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape)) {
            OnClickSettingsButton();
        }
    }

    public void OnClickSettingsButton() {
        if (!turnedOn) {
            turnedOn = true;
            settingsMenu.SetActive(true);
        } else {
            turnedOn = false;
            settingsMenu.SetActive(false);
        }

    }

    public void QuitGame() {
        Application.Quit();
    }

    public void BackToMenu() {
        SceneManager.LoadScene(1); // <-- menu
    }

    public void ReturnToGame() {
        OnClickSettingsButton();
    }
}
