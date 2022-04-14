using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
 
public class DeckSelector : MonoBehaviour
{
    [SerializeField] RectTransform deckList;
    [SerializeField] GridLayoutGroup layout;
    [SerializeField] int padding = 0;

    [Space(30)]
    [SerializeField] TMP_InputField deckName;
    [SerializeField] GameObject PrefabDeck;

    private void OnEnable()
    {
        Resize();
        foreach (Transform child in deckList)
            Destroy(child.gameObject);

        MythosClient.OnDecknamesLoaded += DeckNameLoadedEvent;
        MythosClient.instance.OnRetrieveDeckNames();
    }

    private void DeckNameLoadedEvent(List<string> deckNames) {
        foreach(string DeckName in deckNames) {
            GameObject selector = new GameObject(DeckName, typeof(Image));
            selector.transform.parent = deckList;
            GameObject temp = Instantiate(PrefabDeck, selector.transform);
            temp.GetComponent<SelectDeck>().name = DeckName;
        }
    }

    void Resize()
    {
        layout.constraintCount = 1;
        layout.padding.left = padding;
        layout.padding.right = padding;
        layout.padding.top = padding;
        layout.spacing = new Vector2(padding, padding);
        layout.cellSize = new Vector2(deckList.rect.width - (padding * 2), 100);
    }

    public void ButtonNewDeck()
    {
        if (deckName.text.Equals(""))
            return;

        Menu.instance.OpenEditor();
        DeckEditor.instance.SetDeckName(deckName.text);
    }
}
