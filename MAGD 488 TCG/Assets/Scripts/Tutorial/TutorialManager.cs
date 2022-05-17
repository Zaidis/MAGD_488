using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TutorialManager : MonoBehaviour
{

    [TextArea()]
    public List<string> tooltipTexts = new List<string>();
    [SerializeField]int count = 0;
    public Tooltip tooltip;

    public List<GameObject> arrows = new List<GameObject>();
    public GameObject myCreature, enemyCreature1, enemyCreature2, myArtifact, card;
    public GameObject button1, button2, button3;
    public void ResetArrows() {
        for(int i = 0; i < arrows.Count; i++) {
            if(arrows[i] != null) {
                arrows[i].SetActive(false);
            }
        }
    }

    public void ContinueThroughTutorial() {
        ResetArrows();
        tooltip.description.text = tooltipTexts[count];
        if(arrows[count] != null) {
            arrows[count].SetActive(true);
        }
        count++;

        if(count == 10) {
            myCreature.SetActive(true);
        }

        if(count == 13) {
            myArtifact.SetActive(true);
        }

        if(count == 15) {
            button1.SetActive(true);
            button2.SetActive(true);
            button3.SetActive(true);
        }

        if(count == 18) {
            button1.SetActive(false);
            button2.SetActive(false);
            button3.SetActive(false);
        }
        if(count == 19) {
            enemyCreature1.SetActive(true);
        }
        if(count == 21) {
            enemyCreature2.SetActive(true);
        }
        if(count == 28) {
            card.SetActive(true);
        }
    }

    public void ExitTutorial() {
        SceneManager.LoadScene(1);
    }
}
