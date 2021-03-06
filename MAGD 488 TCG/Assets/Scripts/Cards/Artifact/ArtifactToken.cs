using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
public class ArtifactToken : Token, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler {
    [Header("Artifact Token Variables")]
    public Artifact artifact;
    public TextMeshPro healthText;

    public bool hasAbility;
    public bool castedAbility;

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

    public void ResetToken() {
        castedAbility = false;
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

        if (eventData.button == PointerEventData.InputButton.Left) {
            if ((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {
                if (transform.parent.GetComponent<Tile>().active) {

                    if (GameManager.Singleton.isAttecking) {

                        if (GameManager.Singleton.isHost) {
                            if (!transform.GetComponentInParent<Tile>().hostTile) { //you clicked on a client tile
                                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                                p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), true);

                            }

                        }
                        else {
                            if (transform.GetComponentInParent<Tile>().hostTile) { //you clicked on a host tile
                                Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                                p.UpdateAttackServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(), false);

                            }
                        }

                    }
                    else if (GameManager.Singleton.isUsingAbility) {

                        Player p = GameManager.Singleton._networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
                        p.UpdateTargetedAbilityServerRpc(GameManager.Singleton.selectedCreature.GetComponentInParent<Tile>().GetTileID(), transform.parent.GetComponent<Tile>().GetTileID(),
                            transform.parent.GetComponent<Tile>().hostTile);

                    }


                }
                else { //NOT AN ACTIVE TILE

                    if (GameManager.Singleton.isHost) {
                        if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.hostBoard, transform.parent.GetComponent<Tile>())) {
                            //this is my creature

                            GameManager.Singleton.ArtifactOptionButton(this);
                        }
                    }
                    else {
                        if (GameManager.Singleton.CheckIfMyCreature(GameManager.Singleton.clientBoard, transform.parent.GetComponent<Tile>())) {
                            //this is my creature

                            GameManager.Singleton.ArtifactOptionButton(this);

                        }
                    }


                }
            }



        }
        else if (eventData.button == PointerEventData.InputButton.Right) {
            GameManager.Singleton.panelPopup.UpdatePopup(artifact);
        }
    }

    public void PlayParticles() {
        particles.Play();
    }

    public void UseAbility() {
        artifact.OnAbility(GameManager.Singleton.hostBoard, GameManager.Singleton.clientBoard, transform.parent.GetComponent<Tile>(), GameManager.Singleton.isHost);
        castedAbility = true;

        // GameManager.Singleton.TurnOffOptionsAndUnselect();
        GameManager.Singleton.TurnOffOptionsAndUnselect();

    }





    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.Singleton.cardPopup.UpdateHoverPopup(artifact);
        GameManager.Singleton.cardPopup.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.Singleton.cardPopup.gameObject.SetActive(false);
    }

}