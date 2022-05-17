using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class DirectConnect : MonoBehaviour
{
    [SerializeField] TMP_InputField joinInput;
    [SerializeField] string gameScene;
    (string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] key, string joinCode) serverOut;
    (string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] hostConnectionData, byte[] key) clientOut;
    public async void OnHostRequest() {
        serverOut = await MythosClient.AllocateRelayServerAndGetJoinCode(2);
        Debug.Log("Join Code: " + serverOut.joinCode);
        MythosClient.instance.joinCode = serverOut.joinCode;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(serverOut.ipv4address, serverOut.port, serverOut.allocationIdBytes, serverOut.key, serverOut.connectionData, true);
        Menu.instance.ButtonPregameDeckSelectDirect(true);
    }
    public async void OnJoinRequest() {
        string joinCode = joinInput.text;
        clientOut = await MythosClient.JoinRelayServerFromJoinCode(joinCode);
        MythosClient.instance.joinCode = joinCode;
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(clientOut.ipv4address, clientOut.port, clientOut.allocationIdBytes, clientOut.key, clientOut.connectionData, clientOut.hostConnectionData, true);
        Menu.instance.ButtonPregameDeckSelectDirect(false);    
    }
}
