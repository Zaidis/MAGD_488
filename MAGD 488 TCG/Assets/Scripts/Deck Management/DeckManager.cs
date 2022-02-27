using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private int maxAmountInDeck = 40;
    private int maxAmountIndividualCard = 3;
    public List<Card> allCards;
    public Deck currentDeck;


    public void AddCardToDeck(Card newCard) {

        if (!allCards.Contains(newCard)) {
            return;
        }

        if (currentDeck.myCards.Count < maxAmountInDeck) {
            int count = 0;

            for (int i = 0; i < currentDeck.myCards.Count; i++) {

                if (currentDeck.myCards[i] == newCard) {
                    count++;
                }
            }

            if (count < maxAmountIndividualCard) {
                currentDeck.myCards.Add(newCard);
                Debug.Log("You have added " + newCard.cardName + "!");
            }
            else {
                Debug.Log("Could not add card into deck. You have reached the max amount.");
            }
        } else {
            
            Debug.Log("Could not add card into deck. You cannot exceed <" + maxAmountInDeck.ToString() + "!");
        }
    }

    public void RemoveCardFromDeck(Card card) {
        currentDeck.myCards.Remove(card);
    }

    public void ClearDeck() {
        currentDeck.myCards.Clear();
    }

    public void MakeNewDeck() {
       
        Deck newDeck = new Deck();
        print(newDeck);
        currentDeck = newDeck;
        
        Debug.Log("You are making a new deck!");
    }
}
[System.Serializable]
public class Deck {

    public List<Card> myCards = new List<Card>();
    //Roho

}
