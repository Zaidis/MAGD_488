using UnityEngine.EventSystems;
using UnityEngine;
using TMPro;

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

    }
    public void ButtonDelete()
    {
        
    }
}
