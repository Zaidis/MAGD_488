using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private static GameManager _singleton;
    public static GameManager Singleton { get { return _singleton; } }
    public TextMeshProUGUI TurnStatus;
    public TextMeshProUGUI opponent;
    public Button NextTurn;
    private NetworkManager _networkManager;
    public bool IsHostTurn = true;
    public string yourName;
    private string opponentName; //TODO IMPLEMENT
    void Start() {
        _networkManager = NetworkManager.Singleton;
        if (!_networkManager.IsClient && !_networkManager.IsServer && !_networkManager.IsHost)
            _networkManager.StartHost();
        _singleton = this;        
        TurnStatus = GameObject.Find("TurnStatus").GetComponent<TextMeshProUGUI>();
        NextTurn = GameObject.Find("NextTurn").GetComponent<Button>();
        if (_networkManager.IsHost) {
            TurnStatus.text = "Your Turn!";
            NextTurn.interactable = true;
        } else {
            TurnStatus.text = "Other Player's Turn.";
        }
        opponentName = MythosClient.instance.opponentUserName;
        opponent.text = "Opponent: " + opponentName;

    }
    void Update() {
        //TODO Disallow input and show connecting screen until both clients are connected together
    }
    public void OnNextTurnPressed() {
        TurnStatus.text = "Other User's Turn";
        IsHostTurn = IsHostTurn ? false : true;
        Player player = _networkManager.SpawnManager.GetLocalPlayerObject().GetComponent<Player>();
        player.NextTurnPressed(IsHostTurn);
    }    
}
