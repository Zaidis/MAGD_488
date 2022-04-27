using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class ArtifactToken : Token, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [Header("Artifact Token Variables")]
    public Artifact artifact;
    public TextMeshPro healthText;
   // public int currentHealth;
    public override void ApplyCard() {
        currentHealth = artifact.defaultHealthAmount;
        healthText.text = currentHealth.ToString();

        Art = artifact.cardArt;

    }
    public override void OnPlay() {
        artifact.OnPlay(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard, 
            transform.parent.GetComponent<Tile>());
    }

    public override void UpdateStats() {

        //health
        if (currentHealth > artifact.defaultHealthAmount) {
            //green text because its bigger
            healthText.color = Color.green;
        }
        else if (currentHealth == artifact.defaultHealthAmount) {
            healthText.color = Color.white;
        }
        else {
            healthText.color = Color.red;
        }

        healthText.text = currentHealth.ToString();
    }

    public void OnPointerClick(PointerEventData eventData) {

        if(eventData.button == PointerEventData.InputButton.Left) {
            if (GameManager.Singleton.isHost) {

                if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.hostBoard, transform.parent.GetComponent<Tile>())) {
                    //this is my creature

                    //GameManager.Singleton.CreatureOptionButtons(this, GameManager.Singleton.isHost);

                }
                else {
                    if (GameManager.Singleton.isAttecking) {
                        if (transform.parent.GetComponent<Tile>().active) {
                            

                            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                            p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), true);
                        }
                    }
                }

            } else {
                //not the host 
                if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.clientBoard, transform.parent.GetComponent<Tile>())) {
                    //this is my creature

                    //GameManager.Singleton.CreatureOptionButtons(this, false);

                }
                else {
                    if (GameManager.Singleton.isAttecking) {
                        if (transform.parent.GetComponent<Tile>().active) {
                            

                            Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                            p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), false);
                        }
                    }
                }
            }
        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            GameManager.Singleton.panelPopup.UpdatePopup(artifact);
        }
    }

    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.Singleton.cardPopup.UpdateHoverPopup(artifact);
        GameManager.Singleton.cardPopup.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.Singleton.cardPopup.gameObject.SetActive(false);
    }

}