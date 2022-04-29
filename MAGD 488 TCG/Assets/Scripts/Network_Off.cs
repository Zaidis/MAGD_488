using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
public class Network_Off : MonoBehaviour
{
    // Start is called before the first frame update
    public int menu;
    void Start()
    {
        //if(NetworkManager.Singleton.IsHost)
           // NetworkManager.Singleton.Shutdown();
    }


    public void BackToMenu() {
        SceneManager.LoadScene(menu);
    }
    
}
