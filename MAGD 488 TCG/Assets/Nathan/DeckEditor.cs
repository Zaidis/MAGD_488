using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DeckEditor : MonoBehaviour {
    #region Singleton
    public static DeckEditor instance;
    private void Awake() {
        MythosClient.OnDeckContentLoaded += LoadDeckContentHandler;
        if (instance != null)
            Destroy(instance.gameObject);
        instance = this;
    }
    #endregion

    public new string deckName;
    public List<int> deckID = new List<int>();
    private List<SelectCL> selectCLs = new List<SelectCL>();
    public Card[] cards;

    public int deckListPrefabAmount;

    [SerializeField] RectTransform cardList;
    GridLayoutGroup layoutCL;
    [SerializeField] RectTransform deckList;
    GridLayoutGroup layoutDL;

    [Header("Card List Layout")]
    [SerializeField][Range(1, 10)] int columnCount = 3;
    [SerializeField][Range(0, 99)] int padding = 20;
    [SerializeField][Range(1, 2)] float aspectRation = 1.4f; // 1.4 is a 3.5 by 2.5 aspect ratio

    [Header("UI")]
    [SerializeField] TMP_InputField IFsearch;
    [SerializeField] TextMeshProUGUI Text_name;
    [SerializeField] Button Button_save;

    [Header("DisplayCard")]
    [SerializeField] GameObject GOcard;
    [SerializeField] TextMeshProUGUI cardName;
    [SerializeField] TextMeshProUGUI description;
    [SerializeField] TextMeshProUGUI manaCost;
    [SerializeField] Image cardArt, border;
    [SerializeField] TextMeshProUGUI attack, health;
    [Header("Prefabs")]
    public GameObject PrefabDL;
    private void Start() {        
        cards = Resources.LoadAll("", typeof(Card)).Cast<Card>().ToArray();
        layoutCL = cardList.GetComponent<GridLayoutGroup>();
        layoutDL = deckList.GetComponent<GridLayoutGroup>();
        Resize();
        FillDeckEditorPannel();               
    }
    private void OnEnable() {
        foreach (Transform child in deckList)
            Destroy(child.gameObject);
    }
    public void SetDeckName(string str) {
        deckName = str;
        Text_name.text = deckName;
    }
    private void Update() {
        Resize();
        Button_save.interactable = (deckID.Count > 0);
    }
    public void Search() => FillDeckEditorPannel();
    public void Sort(int sortType) {
        Debug.Log(sortType);
        if (sortType == 0)
            cards = cards.OrderBy(x => x.cardName).ToArray();
        if (sortType == 1)
            cards = cards.OrderBy(x => x.manaCost).ToArray();
        FillDeckEditorPannel();
    }
    void FillDeckEditorPannel() {
        string str = IFsearch.text;
        foreach (Transform child in cardList)
            Destroy(child.gameObject);
        // cards = Resources.FindObjectsOfTypeAll(typeof(Card)) as Card[];
        for (int i = 0; i < cards.Length; i++)
            if (cards[i].cardName.ToLower().Contains(str.ToLower()) || str.Equals(""))
                selectCLs.Add(CreateCard(cards[i]));
    }
    void ResizeCL() {
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
    void ResizeDL() {
        float width = deckList.rect.width - (2 * padding);
        float height = width / 5;

        layoutDL.padding.left = padding;
        layoutDL.padding.right = padding;
        layoutDL.padding.top = padding;
        layoutDL.spacing = new Vector2(0, padding);
        layoutDL.cellSize = new Vector2(width, height);
    }
    void Resize() {
        ResizeCL();
        ResizeDL();
    }

    private void LoadDeckContentHandler(List<int> deck) {
        deckID.Clear();
        foreach(SelectCL cl in selectCLs)
            cl.count = 0;
        for (int i = 0; i < deck.Count; i++) {
            SelectCL cl = selectCLs.First(cl => cl.card.ID == deck[i]);
            cl.OnLoadDeckInteract();
        }
    }

    SelectCL CreateCard(Card card) {
        GameObject selector = new GameObject("Card " + card.name, typeof(Image), typeof(SelectCL));
        selector.transform.SetParent(cardList);

        SelectCL selectCL = selector.GetComponent<SelectCL>();
        selector.GetComponent<Image>().sprite = card.cardArt; // setup art
        selectCL.card = card; // cash selectCL

        return selectCL;
    }
    public void ButtonSave() {
        Debug.Log("Saving Deck");

        if (deckID.Count == 0) {
            Debug.Log("cannot be empty");
            return;
        }

        MythosClient.instance.OnSaveDeck(deckName, deckID.ToArray());

    }
    public void DisplayCard(Card card) {
        GOcard.SetActive(true);
        cardName.text = card.cardName;
        description.text = card.description;
        manaCost.text = card.manaCost.ToString();
        cardArt.sprite = card.cardArt;
        border.sprite = card.cardBorder;

        if(card is Creature c) {
            health.text = c.defaultHealthAmount.ToString();
            attack.text = c.defaultPowerAmount.ToString();
        } else if (card is Artifact a) {
            health.text = a.defaultHealthAmount.ToString();
        }

    }    

    public void IncreaseDeckListScale() { //gives more room

        //affect the bottom
        deckList.offsetMin = new Vector2(deckList.offsetMin.x, deckList.offsetMin.y - 100); 

    }

    public void DecreaseDeckListScale() {

        //affect the bottom
        deckList.offsetMin = new Vector2(deckList.offsetMin.x, deckList.offsetMin.y + 100);

    }

    public void SetDeckListButtonAmount(int num) {
        int temp = deckListPrefabAmount;
        deckListPrefabAmount += num;

        if(temp > deckListPrefabAmount) {
            //we removed a button
            //DecreaseDeckListScale();
            return;
        }

        if(deckListPrefabAmount > 6) {
            IncreaseDeckListScale();
        } 

    }
}
