using System.Collections;
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
    public List<int> deckID = new List<int>();
    public Card[] cards;

    [SerializeField] RectTransform cardList;
    GridLayoutGroup layoutCL;
    [SerializeField] RectTransform deckList;
    GridLayoutGroup layoutDL;

    [Header("Card List Layout")]
    [SerializeField][Range(1,10)] int columnCount = 3;
    [SerializeField][Range(0,99)] int padding = 20;
    [SerializeField][Range(1,2)] float aspectRation = 1.4f; // 1.4 is a 3.5 by 2.5 aspect ratio

    private void OnEnable()
    {
        layoutCL = cardList.GetComponent<GridLayoutGroup>();
        layoutDL = deckList.GetComponent<GridLayoutGroup>();
        Resize();

        EraseData(); // clear table
<<<<<<< Updated upstream
        cards = Resources.FindObjectsOfTypeAll(typeof(Card)) as Card[];
        MythosClient.OnDeckContentLoaded += LoadDeckContentHandler;
        MythosClient.instance.OnRetrieveDeckContent(name);     
=======
        cards = Resources.LoadAll<Card>("/");
        StartCoroutine(GetContent()); // 
        
>>>>>>> Stashed changes

        for (int i = 0; i < cards.Length; i++)
            CreateCard(cards[i]);

    }
    private void Update()
    {
        Resize();
        if (Input.GetKeyDown(KeyCode.S))
            ButtonSave();
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

    private void LoadDeckContentHandler(List<int> deck) {
        deckID = deck;
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
        selector.transform.SetParent(cardList);

        SelectCL selectCL = selector.GetComponent<SelectCL>();
        selector.GetComponent<Image>().sprite = card.cardArt; // setup art
        selectCL.card = card; // cash selectCL

        return selectCL;
    }
    public void ButtonSave()
    {
        MythosClient.instance.OnSaveDeck(name, deckID.ToArray());
    }
}
