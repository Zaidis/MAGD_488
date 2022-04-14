using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;
using System;

public class Player : NetworkBehaviour {
    private NetworkManager _networkManager;
    private NetworkObject _networkObject;
    //public NetworkVariable<Token> hostTiles = new NetworkVariable<Token>();
    //public NetworkVariable<Token> clientTiles = new NetworkVariable<Token>();

    void Start() {
        _networkObject = GetComponent<NetworkObject>();
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkObject.IsLocalPlayer) {
            GameObject CL;
            if (_networkManager.IsHost)
               CL = GameObject.Find("HostCameraLocation");
            else
                CL = GameObject.Find("ClientCameraLocation");
            Camera.main.transform.SetPositionAndRotation(CL.transform.position, CL.transform.rotation);
            transform.SetPositionAndRotation(CL.transform.position, CL.transform.rotation);
            Camera.main.transform.parent = transform;
            Debug.Log("Player Spawned!");
        }
    }
    public void NextTurnPressed(bool isHostTurn) {
        if (_networkManager.IsHost) {
            UpdateTurnClientRpc(isHostTurn);
        } else {
            UpdateTurnServerRpc(isHostTurn);
        }
    }    
    public void PlaceCard(int cardID, int x, int y) { //<----- NEEDS TO CALL THIS
        if (_networkManager.IsHost) {
            GameManager.Singleton.PlaceCard(true, cardID, x, y);
            UpdatePlaceCardClientRpc(x, y, cardID);
        } else {
            GameManager.Singleton.PlaceCard(false, cardID, x, y);
            UpdatePlaceCardServerRpc(x, y, cardID);
        }
    }
   
    #region Placing Cards
    /*
     * We need to know the empty token prefab to add in the info
     * WE need to know the location of where the card is placed. <---- Do this first
     */
    [ServerRpc]
    private void UpdatePlaceCardServerRpc(int x, int y, int cardID) {
        GameManager.Singleton.PlaceCard(false, cardID, x, y);
    }

    [ClientRpc]
    private void UpdatePlaceCardClientRpc(int x, int y, int cardID) {
        GameManager.Singleton.PlaceCard(true, cardID, x, y);
    }
    #endregion*/
    
    #region Attacking
    [ServerRpc]
    private void UpdateAttackServerRpc(bool isHostTurn) {
        
        

    }

    [ClientRpc]
    private void UpdateClientClientRpc(bool isHostTurn) {

    }
    #endregion
    #region Turns

    [ServerRpc]
    private void UpdateTurnServerRpc(bool isHostTurn) {
        GameManager.Singleton.IsHostTurn = isHostTurn;
        if (_networkManager.IsHost) {
            if (isHostTurn) {
                GameManager.Singleton.TurnStatus.text = "Your Turn!";
                GameManager.Singleton.NextTurn.interactable = true;
            }
        }
        else {
            if (!isHostTurn) {
                GameManager.Singleton.TurnStatus.text = "Your Turn!";
                GameManager.Singleton.NextTurn.interactable = true;
            }
        }
    }

    [ClientRpc]
    void UpdateTurnClientRpc(bool isHostTurn) {
        GameManager.Singleton.IsHostTurn = isHostTurn;
        if (_networkManager.IsHost) {
            if (isHostTurn) {
                GameManager.Singleton.TurnStatus.text = "Your Turn!";
                GameManager.Singleton.NextTurn.interactable = true;
            }
        } else {
            if (!isHostTurn) {
                GameManager.Singleton.TurnStatus.text = "Your Turn!";
                GameManager.Singleton.NextTurn.interactable = true;
            }                
        }
    }
    #endregion
}
