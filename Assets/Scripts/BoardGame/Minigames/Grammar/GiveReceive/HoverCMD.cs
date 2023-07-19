using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class HoverCMD : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string text;
    void Start() {
        text = transform.GetChild(0).GetComponent<TextMeshProUGUI>().text;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = ">" + text;
    }

    public void OnPointerExit(PointerEventData eventData) {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = text;
    }
}
