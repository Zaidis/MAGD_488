using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public class UICard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    public float defaultRotation_Z;
    public float defaultPosition_Y;
    public int sortingOrder;

    private RectTransform myTransform;
    private void Start() {
        myTransform = GetComponent<RectTransform>();
    }

    //When you hover over card. REQUIRES GRAPHIC RAYCASTER 
    public void OnPointerEnter(PointerEventData eventData) {

        Debug.Log("Hovering over card");
        GetComponent<Canvas>().sortingOrder = 20;

        myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x, 130); //fix later

        myTransform.rotation = Quaternion.Euler(0, 0, 0);


    }

    //when you stop hovering over card
    public void OnPointerExit(PointerEventData eventData) {

        GetComponent<Canvas>().sortingOrder = sortingOrder;
        myTransform.rotation = Quaternion.Euler(0, 0, defaultRotation_Z);

        myTransform.anchoredPosition = new Vector2(myTransform.anchoredPosition.x, defaultPosition_Y);

    }
}
