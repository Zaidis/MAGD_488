using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using TMPro;

public class Player : NetworkBehaviour {
    private NetworkManager _networkManager;
    private void Awake() {
        DontDestroyOnLoad(this);
    }
    void Start() {
        _networkManager = FindObjectOfType<NetworkManager>();
        if (_networkManager.IsClient) {
            Debug.Log("Player Spawned!");
        }     
    }
    void Update() {

    }
    public void NextTurnPressed(bool isHostTurn) {
        UpdateTurnClientRpc(isHostTurn);
    }
    [ClientRpc]
    void UpdateTurnClientRpc(bool isHostTurn) {
        Debug.Log("Updating Turn");
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
