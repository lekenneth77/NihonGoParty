using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour, IDropHandler
{
    public static event Action Dropped;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag || gameObject.GetComponent<DnDInfo>().onMeOnThem) { return; }
        Debug.Log("Hey!");
        GameObject dragObj = eventData.pointerDrag;
        dragObj.transform.position = transform.position;
        //basically marks this dropspot as used!
        gameObject.GetComponent<DnDInfo>().onMeOnThem = dragObj.GetComponent<DnDInfo>();
        dragObj.GetComponent<DnDInfo>().onMeOnThem = gameObject.GetComponent<DnDInfo>();
        Color temp = gameObject.GetComponent<Image>().color;
        temp.a = 0f;
        gameObject.GetComponent<Image>().color = temp;
        Dropped?.Invoke();
    }
}
