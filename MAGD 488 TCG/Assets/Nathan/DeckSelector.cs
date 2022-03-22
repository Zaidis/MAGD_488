using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckSelector : MonoBehaviour
{
    [SerializeField] RectTransform deckList;
    [SerializeField] GridLayoutGroup layout;
    [SerializeField] int padding = 0;

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
            GameObject selector = new GameObject(DeckName, typeof(Image), typeof(SelectDeck));
            selector.transform.parent = deckList;
            selector.GetComponent<SelectDeck>().name = DeckName;
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
}
