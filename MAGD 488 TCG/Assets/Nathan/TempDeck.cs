using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TempDeck : MonoBehaviour
{

    public static TempDeck instance;

    public List<int> deckID = new List<int>();
    public bool usingCustomDeck;
    private void Awake() {
        if(instance == null) {
            instance = this;
        } else {
            Destroy(this.gameObject);
        }
        DontDestroyOnLoad(this.gameObject);
    }

    public void AddListToTemporaryDeck(List<int> newDeck) {

        for(int i = 0; i < newDeck.Count; i++) {
            deckID.Add(newDeck[i]);
        }

    }


}
