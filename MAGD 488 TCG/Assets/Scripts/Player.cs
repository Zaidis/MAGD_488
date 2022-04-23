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
    
    [ClientRpc]
    public void UpdatePlaceCardClientRpc(bool sidePlacedOn, int id, int cardID) {
        GameManager.Singleton.PlaceCard(sidePlacedOn, cardID, id);
    }
    [ServerRpc]
    public void UpdatePlaceCardServerRpc(bool sidePlacedOn, int id, int cardID) {
        UpdatePlaceCardClientRpc(sidePlacedOn, id, cardID);
    }

    [ClientRpc]
    public void UpdateAttackClientRpc(int attackerID, int attackedID, bool attackingFromHostSide) {
        GameManager.Singleton.Attack(attackerID, attackedID, attackingFromHostSide);
    }
    [ServerRpc]
    public void UpdateAttackServerRpc(int attackerID, int attackedID, bool attackingFromHostSide) {
        UpdateAttackClientRpc(attackerID, attackedID, attackingFromHostSide);
    }

    [ClientRpc]
    public void UpdateHealthClientRpc(int hostAmount, int clientAmount) {
        GameManager.Singleton.AffectHealthValues(hostAmount, clientAmount);
    }
    [ServerRpc]
    public void UpdateHealthServerRpc(int hostAmount, int clientAmount) {
        UpdateHealthClientRpc(hostAmount, clientAmount);
    }

    [ServerRpc]
    public void UpdateTurnServerRpc(bool isHostTurn) {
        UpdateTurnClientRpc(isHostTurn);
    }
    [ClientRpc]
    private void UpdateTurnClientRpc(bool isHostTurn) {
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
}
