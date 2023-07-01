using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class WordOrderWord : MonoBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    private Vector3 originalPosition;
    private MoveObject moveObj;
    public string word;
    public int length;
    public WordOrderWord next;
    public WordOrderWord prev;
    public static event Action remove;
    public bool bbyRUDown;

    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public bool allowDrag;
    public static bool mrWorldwideDrag;

    // Start is called before the first frame update
    void Start() {
        originalPosition = transform.localPosition;
        moveObj = GetComponent<MoveObject>();
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
    }

    public void ClickedOn() {
        GetComponent<Button>().enabled = false;
        moveObj.SetTargetAndMove(WordOrder.nextPos);
        //gotClicked?.Invoke(gameObject);
    }

    public void Move(Vector3 pos, bool teleport) { 
        if (teleport) { 
            transform.localPosition = pos;
        } else {
            moveObj.SetTargetAndMove(pos);
        }
    }

    public void ResetWord() {
        moveObj.SetTargetAndMove(originalPosition);
        GetComponent<Button>().enabled = true;
    }


    public void ChangeWord(string s) {
        word = s;
        length = s.Length;
    }

    public void SetDrop(bool drop) {
        transform.GetChild(length).gameObject.SetActive(drop);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!mrWorldwideDrag || !allowDrag) { return; }
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;

        if (!bbyRUDown) { return; }
        bbyRUDown = false;
        prev.next = next;
        next.prev = prev;

        //adjust them
        AdjustFront(this);

        next = null; 
        prev = null;
        SetDrop(false);
        remove?.Invoke();
    }

    public void AdjustFront(WordOrderWord obj) {

        WordOrderWord next = obj.next;
        while (next != null)
        {
            MoveWord(next.gameObject, next.prev.transform.position + new Vector3(next.prev.length * 100f + 50f, 0f));
            next = next.next;
        }
    }

    public void MoveWord(GameObject obj, Vector3 pos) { 
        if (pos.x > 1800f || pos.x + (100 * (obj.GetComponent<WordOrderWord>().length - 1)) > 1800f) {
            obj.transform.position = new Vector3(160f, pos.y - 175f);
        } else { 
            obj.transform.position = pos;
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!mrWorldwideDrag || !allowDrag) { return; }
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

  

}
