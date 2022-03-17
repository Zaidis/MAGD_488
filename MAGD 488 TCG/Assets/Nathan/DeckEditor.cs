using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckEditor : MonoBehaviour
{
    #region Singleton
    public static DeckEditor instance;
    private void Awake()
    {
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }
    #endregion

    public new string name;
    public List<int> owned = new List<int>();
    public List<int> deckID = new List<int>();
    public List<Card> cards = new List<Card>();

    [SerializeField] RectTransform cardList;
    GridLayoutGroup layoutCL;
    [SerializeField] RectTransform deckList;
    GridLayoutGroup layoutDL;

    [Header("Card List Layout")]
    [SerializeField][Range(1,10)] int columnCount = 3;
    [SerializeField][Range(0,99)] int padding = 20;
    [SerializeField][Range(1,2)] float aspectRation = 1.4f; // 1.4 is a 3.5 by 2.5 aspect ratio

    private void Start()
    {
        layoutCL = cardList.GetComponent<GridLayoutGroup>();
        layoutDL = deckList.GetComponent<GridLayoutGroup>();
        Resize();

        EraseData();

        // load cards

        // load deck first before create SelectCl and selectDl
        // Create SelectDL from the loaded deck

        for (int i = 0; i < cards.Count; i++)
            CreateCard(cards[i]);

    }

    void ResizeCL()
    {
        layoutCL.constraintCount = columnCount;
        layoutCL.padding.left = padding;
        layoutCL.padding.right = padding;
        layoutCL.padding.top = padding;
        layoutCL.spacing = new Vector2(padding, padding);

        float paddingSum = (columnCount + 1) * padding;
        float width = (cardList.rect.width - paddingSum) / columnCount;
        float height = width * aspectRation;
        layoutCL.cellSize = new Vector2(width, height);

        float numOfRows = cardList.childCount / columnCount;
        if (cardList.childCount % columnCount != 0) numOfRows++;

        cardList.sizeDelta = new Vector2(cardList.sizeDelta.x, (padding * (numOfRows + 1)) + (height * numOfRows));

    }
    void ResizeDL()
    {
        float width = deckList.rect.width - (2 * padding);
        float height = width / 5;

        layoutDL.padding.left = padding;
        layoutDL.padding.right = padding;
        layoutDL.padding.top = padding;
        layoutDL.spacing = new Vector2(0,padding);
        layoutDL.cellSize = new Vector2(width, height);
    }
    void Resize()
    {
        ResizeCL();
        ResizeDL();
    }



    void EraseData()
    {
        foreach (Transform child in cardList)
            Destroy(child.gameObject);

        foreach (Transform child in deckList)
            Destroy(child.gameObject);
    }

    SelectCL CreateCard(Card card)
    {
        GameObject selector = new GameObject("Card " + card.name, typeof(Image), typeof(SelectCL));
        selector.transform.parent = cardList;

        SelectCL selectCL = selector.GetComponent<SelectCL>();
        selector.GetComponent<Image>().sprite = card.cardArt; // setup art
        selectCL.card = card; // cash selectCL

        int count = 0; 
        for (int i = 0; i < owned.Count; i++) // go through players owned cards
            if (card.ID == owned[i]) count++; // add count if it is the same
        for (int i = 0; i < deckID.Count; i++) // go through deck
            if (deckID[i] == card.ID) // remove count if card is in deck
                count--;
        selectCL.owned = count;

        return selectCL;
    }
}
