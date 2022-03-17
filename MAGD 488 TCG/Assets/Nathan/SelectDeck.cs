using UnityEngine.EventSystems;
using UnityEngine;

public class SelectDeck : MonoBehaviour, IPointerClickHandler
{
    public new string name;
    public void OnPointerClick(PointerEventData eventData)
    {
        Menu.instance.OpenEditor();
        DeckEditor.instance.name = name;
    }
}