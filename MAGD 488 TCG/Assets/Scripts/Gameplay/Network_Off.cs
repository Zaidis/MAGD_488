using UnityEngine;
using UnityEngine.SceneManagement;
public class Network_Off : MonoBehaviour
{
    public string menu;
    public void BackToMenu() {
        SceneManager.LoadScene(menu);
    }    
}
