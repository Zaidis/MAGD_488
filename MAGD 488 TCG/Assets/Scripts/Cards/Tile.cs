using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler {
    public GameObject token;
    public Transform spawnLocation;
    public bool active;
    [SerializeField] private int tileID; //each tile is unique
    

    public void Start() {
        GetComponent<MeshRenderer>().material = GameManager.Singleton.m_default;
    }
    public void SetToken(GameObject token) {
        this.token = token;
        token.transform.position = spawnLocation.position;
        token.transform.parent = transform;
    }

    public int GetTileID() {
        return tileID;
    }

    public void DealtDamage(int damageAmount) {
        Token t = token.GetComponent<CreatureToken>();
        t.currentHealth -= damageAmount;
        
        if(t.currentHealth <= 0) {
            //destroy token
            Destroy(token);
        } else {
            token.GetComponent<CreatureToken>().UpdateStats();
        }
        

    }

    public void ChangeMaterial(Material mat) {
        GetComponent<MeshRenderer>().material = mat;
    }
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("I clicked on a tile!");
        if (token == null) {
            if (GameManager.Singleton.needsToSelectTile) { //if you need to select a tile (you are playing a card)
                                                           //we have selected the tile

                if (GameManager.Singleton.selectedCard.type == cardType.creature) {

                    Creature c = (Creature)GameManager.Singleton.selectedCard;
                    GameObject t = Instantiate(GameManager.Singleton.CreatureTokenPrefab);
                    t.GetComponent<CreatureToken>().creature = c;
                    t.GetComponent<CreatureToken>().ApplyCard();
                    SetToken(t);

                    GameManager.Singleton.AffectCurrentMana(c.manaCost * -1);

                    Hand.instance.RemoveCardFromHand();

                    GameManager.Singleton.ResetSelectedCard();

                }
                else if (GameManager.Singleton.selectedCard.type == cardType.artifact) {
                    Artifact a = (Artifact)GameManager.Singleton.selectedCard;
                    GameObject t = Instantiate(GameManager.Singleton.ArtifactTokenPrefab);
                    t.GetComponent<ArtifactToken>().artifact = a;
                    t.GetComponent<ArtifactToken>().ApplyCard();
                    SetToken(t);

                    GameManager.Singleton.AffectCurrentMana(a.manaCost * -1);

                    Hand.instance.RemoveCardFromHand();

                    GameManager.Singleton.ResetSelectedCard();
                }
            }
        }/* else {
            if (GameManager.Singleton.isAttecking) {

                if (this.active) {
                    //attack this
                    GameManager.Singleton.selectedCreature.AttackWithToken(this);

                }


            }
        } */
    }
}
