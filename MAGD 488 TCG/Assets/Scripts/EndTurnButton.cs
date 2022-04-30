using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
public class EndTurnButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{

    public bool interactable;
    public TextMeshPro buttonText;
    

    [SerializeField] private Material m_onHover; //your turn and hovering
    [SerializeField] private Material m_on; //your turn
    [SerializeField] private Material m_off; //enemy turn

    /// <summary>
    /// Only called when it is your turn!
    /// </summary>
    public void ActivateButton() {
        buttonText.text = "End Turn";
        gameObject.GetComponent<MeshRenderer>().material = m_on;
        interactable = true;
    }

    public void DeactivateButton() {
        buttonText.text = "Enemy Turn";
        gameObject.GetComponent<MeshRenderer>().material = m_off;
        interactable = false;
    }

    public void OnPointerEnter(PointerEventData eventData) {

        if ((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {

            //it is your turn

            gameObject.GetComponent<MeshRenderer>().material = m_onHover;

        }
    }

    public void OnPointerExit(PointerEventData eventData) {

        if ((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {

            //it is your turn

            gameObject.GetComponent<MeshRenderer>().material = m_on;

        } 

    }

    public void OnPointerClick(PointerEventData eventData) {

        if ((GameManager.Singleton.isHost && GameManager.Singleton.IsHostTurn) || (!GameManager.Singleton.isHost && !GameManager.Singleton.IsHostTurn)) {

            //it is your turn

            //gameObject.GetComponent<MeshRenderer>().material = m_onHover;
            GameManager.Singleton.OnNextTurnPressed();
            interactable = false;
        }

    }
}
