using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

//more appropriately it should be punch but whatever
public class RPGCircle : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public bool disable;
    public bool giver;
    public bool amLeft;
    public bool alreadyClicked;
    public RPGPlayerPhase phase;
    public TextMeshProUGUI aboveText;
    public event Action<bool, bool> GotClicked;
    
    public void ChangeText(string s) {
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        if (disable) { return; }

        if (alreadyClicked) {
            aboveText.text = "<s>Giver</s>";
        }
        else {
            aboveText.text = phase.countClicks == 0 ? "Giver" : "Receiver";
            aboveText.gameObject.SetActive(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData) {
        if (disable) { return; }

        if (alreadyClicked) {
            aboveText.text = "Giver";
        } else {
            aboveText.gameObject.SetActive(false);
        }
    }

    public void OnClick() {
        if (disable) { return; }

        if (alreadyClicked) {
            alreadyClicked = false;
            phase.countClicks--;
            aboveText.gameObject.SetActive(false);
        }
        else {
            alreadyClicked = true;
            GotClicked?.Invoke(amLeft, giver);
        }
    }
}
