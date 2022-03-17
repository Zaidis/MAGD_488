using System.Collections;
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
        for (int i = 0; i < MythosClient.instance.deckNames.Count; i++)
            AddDeck(MythosClient.instance.deckNames[i]);
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
    void AddDeck(string name)
    {
        GameObject selector = new GameObject(name, typeof(Image), typeof(SelectDeck));
        selector.transform.parent = deckList;
        selector.GetComponent<SelectDeck>().name = name;
    }
}
