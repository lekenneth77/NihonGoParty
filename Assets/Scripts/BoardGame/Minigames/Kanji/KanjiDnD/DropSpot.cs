using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DropSpot : MonoBehaviour, IDropHandler
{
    public static event Action Dropped;

    public void OnDrop(PointerEventData eventData)
    {
        if (!eventData.pointerDrag || gameObject.GetComponent<DnDInfo>().onMeOnThem || !DragNDrop.allowDrag) { return; }
        GameObject dragObj = eventData.pointerDrag;
        dragObj.transform.position = transform.position;
        //basically marks this dropspot as used!
        gameObject.GetComponent<DnDInfo>().onMeOnThem = dragObj.GetComponent<DnDInfo>();
        dragObj.GetComponent<DnDInfo>().onMeOnThem = gameObject.GetComponent<DnDInfo>();
        Dropped?.Invoke();
        gameObject.SetActive(false);
    }
}
