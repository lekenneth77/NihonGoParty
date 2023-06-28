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
    public TextMeshProUGUI word;
    public int length;
    public WordOrderWord next;
    public WordOrderWord prev;
    //public static event Action<GameObject> gotClicked;

    public Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    public bool allowDrag;

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
        word.text = s;
        length = s.Length;
    }

    public void SetDrop(bool drop) {
        transform.GetChild(length).gameObject.SetActive(drop);
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!allowDrag) { return; }
        canvasGroup.alpha = .6f;
        canvasGroup.blocksRaycasts = false;
        GameObject dragObj = eventData.pointerDrag;
    }

    public void OnDrag(PointerEventData eventData)
    {
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
    }

  

}
