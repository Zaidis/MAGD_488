using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TabInputField : MonoBehaviour
{
    [SerializeField] public TMP_InputField[] inputFields;
    public Button enter;
    public int InputSelected;
    private bool usingFields = false;
    private void Update() {
        if (usingFields) {
            if (Input.GetKeyDown(KeyCode.Tab)) {
                InputSelected++;
                if (InputSelected > inputFields.Length-1) InputSelected = 0;
                inputFields[InputSelected].Select();
            }
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
                enter.onClick.Invoke();
        }
    }
    public void UsingField(int i) {
        InputSelected = i;
        usingFields = true;
    }
    public void StoppedUsingField() { usingFields = false; }
}
