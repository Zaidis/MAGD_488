using UnityEngine;
using UnityEngine.UI;
public class AutoReenableButton : MonoBehaviour {
    private Button button;
    void Awake() {
        button = gameObject.GetComponent<Button>();
    }
    void OnEnable() {
        button.interactable = true;
    }
}
