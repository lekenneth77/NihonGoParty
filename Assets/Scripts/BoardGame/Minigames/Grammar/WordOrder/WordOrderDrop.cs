using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordOrderDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log(transform.position.x);
        GameObject dragObj = eventData.pointerDrag;
        dragObj.transform.position = transform.position + new Vector3(75f, 0f);
        dragObj.GetComponent<WordOrderWord>().SetDrop(true);

        //get this guys next
        WordOrderWord next = transform.parent.GetComponent<WordOrderWord>().next;
        transform.parent.GetComponent<WordOrderWord>().prev = dragObj.GetComponent<WordOrderWord>();
        dragObj.GetComponent<WordOrderWord>().next = next;
        dragObj.GetComponent<WordOrderWord>().prev = transform.parent.GetComponent<WordOrderWord>();
        transform.parent.GetComponent<WordOrderWord>().next = dragObj.GetComponent<WordOrderWord>();


        next = dragObj.GetComponent<WordOrderWord>().next;
        while (next != null) {
            next.transform.position = next.transform.position + new Vector3(dragObj.GetComponent<WordOrderWord>().length * 100f + 50f, 0f);
            next = next.next;
        }
    }

}
