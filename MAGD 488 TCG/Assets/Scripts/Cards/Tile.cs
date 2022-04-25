using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerClickHandler {
    public GameObject token;
    public Transform hostSpawnLocation;
    public Transform clientSpawnLocation;
    public bool hostTile;
    public bool meleeTile;
    public bool active;
    [SerializeField] private int tileID; //each tile is unique
    

    public void Start() {
        GetComponent<MeshRenderer>().material = GameManager.Singleton.m_default;
    }
    public void SetToken(GameObject token) {
        this.token = token;
        
        //new material texture
        Renderer r = token.GetComponent<Token>().cardArtHolder.GetComponent<Renderer>();
        
        r.material = new Material(GameManager.Singleton.defaultShader);
        r.material.mainTexture = token.GetComponent<Token>().Art.texture;

        if (GameManager.Singleton.isHost) {
            token.transform.position = hostSpawnLocation.position;
        } else {
            token.transform.position = clientSpawnLocation.position;
            token.transform.rotation = Quaternion.Euler(270, 0, 0);
        }
        
        token.transform.parent = transform;
        token.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);

        //after setting the token, check to see if there are any OnPlay calls from the creature/artifact

        token.GetComponent<Token>().OnPlay();


    }

    public int GetTileID() {
        return tileID;
    }

    public void DealtDamage(int damageAmount) {
        Token t = token.GetComponent<Token>();
        t.currentHealth -= damageAmount;
        if(t is CreatureToken c){
            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
            if (c.creature.myAttributes.Contains(attributes.lifesteal)) {
                //make this later
                if (hostTile) { //host health goes up
                    p.UpdateHealthServerRpc(c.currentAttack, 0);

                } else { //client health goes up
                    p.UpdateHealthServerRpc(0, c.currentAttack);
                }
            }
            if (c.creature.myAttributes.Contains(attributes.thorn)) { //when attacked, deals damage to attacker
                if (hostTile) { //deals 1 damage to the opponent

                    p.UpdateHealthServerRpc(0, c.currentAttack * -1);
                }
                else { //client health goes up
                    p.UpdateHealthServerRpc(c.currentAttack * -1, 0);
                    
                }
            }
        } 
        if(t.currentHealth <= 0) {
            //destroy token
            Destroy(token);
        } else {
            t.UpdateStats();
        }
        

    }

    public void ChangeMaterial(Material mat) {
        if(token != null)
            token.GetComponent<Token>().ChangeMaterial(mat);
        //GetComponent<MeshRenderer>().material = mat;
    }
    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("I clicked on a tile!");
        if (token == null) {
            if (GameManager.Singleton.needsToSelectTile) { //if you need to select a tile (you are playing a card)
                                                           //we have selected the tile

                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                p.UpdatePlaceCardServerRpc(GameManager.Singleton._networkManager.IsHost, GetTileID(), GameManager.Singleton.selectedCard.ID);
                Hand.instance.RemoveCardFromHand();
                GameManager.Singleton.ResetSelectedCard();
                /*if (GameManager.Singleton.selectedCard.type == cardType.creature) {

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
                } */
            }
        }
    }
}
