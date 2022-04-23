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
    public void PlaceCard(int cardID, int id) { //<----- NEEDS TO CALL THIS
        if (_networkManager.IsHost) {
            GameManager.Singleton.PlaceCard(true, cardID, id);
            UpdatePlaceCardClientRpc(id, cardID);
        } else {
            GameManager.Singleton.PlaceCard(false, cardID, id);
            UpdatePlaceCardServerRpc(id, cardID);
        }
    }
   
    #region Placing Cards
    /*
     * We need to know the empty token prefab to add in the info
     * WE need to know the location of where the card is placed. <---- Do this first
     */
    [ServerRpc]
    private void UpdatePlaceCardServerRpc(int id, int cardID) {
        GameManager.Singleton.PlaceCard(false, cardID, id);
    }

    [ClientRpc]
    private void UpdatePlaceCardClientRpc(int id, int cardID) {
        GameManager.Singleton.PlaceCard(true, cardID, id);
    }
    #endregion*/
    
    #region Attacking

    public void Attack(int attackerID, int attackedID) {
        if (GameManager.Singleton.isHost) {
            GameManager.Singleton.Attack(attackerID, attackedID, true);
            UpdateAttackClientRpc(attackerID, attackedID);
        } else {
            GameManager.Singleton.Attack(attackerID, attackedID, false);
            UpdateAttackServerRpc(attackerID, attackedID);
        }
    }


    [ServerRpc]
    private void UpdateAttackServerRpc(int attackerID, int attackedID) {

        GameManager.Singleton.Attack(attackerID, attackedID, false);

    }

    [ClientRpc]
    private void UpdateAttackClientRpc(int attackerID, int attackedID) {
        GameManager.Singleton.Attack(attackerID, attackedID, true);
    }


    public void UpdateHealth(bool isHost, int hostAmount, int clientAmount) {
        if (isHost) {
            GameManager.Singleton.AffectHealthValues(hostAmount, clientAmount);
            UpdateHealthClientRpc(hostAmount, clientAmount);
        } else {
            GameManager.Singleton.AffectHealthValues(hostAmount, clientAmount);
            UpdateHealthServerRpc(hostAmount, clientAmount);
        }
    }

    [ServerRpc]
    private void UpdateHealthServerRpc(int hostAmount, int clientAmount) {

        GameManager.Singleton.AffectHealthValues(hostAmount, clientAmount);
    }

    [ClientRpc]
    private void UpdateHealthClientRpc(int hostAmount, int clientAmount) {
        Debug.Log("Test test test!");
        GameManager.Singleton.AffectHealthValues(hostAmount, clientAmount);
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
