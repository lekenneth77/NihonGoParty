using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

public class KatakanaCollider : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    // Start is called before the first frame update
    private Rigidbody2D rbody;
    public float constantSpeed;
    public static bool disable;
    public bool nono;
    public bool correctOne;
    public static event Action<bool> GotClicked;

    void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        rbody.velocity = new Vector2(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f)).normalized;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        rbody.velocity = constantSpeed * (rbody.velocity.normalized);
    }

    public void ClickedOn() {
        if (disable) { return; }
        GotClicked?.Invoke(correctOne);
        if (correctOne) {
            transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (disable) { return; }
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.blue;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (nono) { return; }
        transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.black;
    }
}
