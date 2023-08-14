using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class WordOrderDrop : MonoBehaviour, IDropHandler
{
    public static event Action drop;

    public void OnDrop(PointerEventData eventData)
    {
        GameObject dragObj = eventData.pointerDrag;
        WordOrderWord newMid = dragObj.GetComponent<WordOrderWord>();
        WordOrderWord thisOne = transform.parent.GetComponent<WordOrderWord>();

        newMid.MoveWord(dragObj, transform.parent.localPosition + new Vector3(100f * (thisOne.length) + 50f, 0f, 0f));

        //get this guys next
        WordOrderWord next = thisOne.next;
        next.prev = newMid;
        thisOne.next = newMid;
        newMid.next = next;
        newMid.prev = thisOne;

        newMid.bbyRUDown = true;
        newMid.SetDrop(true);

        newMid.AdjustFront(newMid);
        drop?.Invoke();
    }
}
