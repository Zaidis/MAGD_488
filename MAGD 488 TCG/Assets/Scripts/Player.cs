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
    void Update() {

    }
    public void NextTurnPressed(bool isHostTurn) {
        if (_networkManager.IsHost) {
            UpdateTurnClientRpc(isHostTurn);
        } else {
            UpdateTurnServerRpc(isHostTurn);
        }
    }
    [ServerRpc]
    private void UpdateTurnServerRpc(bool isHostTurn) {
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
}
