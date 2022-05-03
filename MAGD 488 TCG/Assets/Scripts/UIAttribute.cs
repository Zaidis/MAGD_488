using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class UIAttribute : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    

    [TextArea()]
    public string title, description;


    public void OnPointerEnter(PointerEventData eventData) {
        GameManager.Singleton.tooltip.gameObject.SetActive(true);
        GameManager.Singleton.tooltip.title.text = title;
        GameManager.Singleton.tooltip.description.text = description;
    }

    public void OnPointerExit(PointerEventData eventData) {
        GameManager.Singleton.tooltip.gameObject.SetActive(false);
    }
}
