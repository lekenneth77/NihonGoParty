using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KatakanaCollider : MonoBehaviour
{
    // Start is called before the first frame update
    private Rigidbody2D rbody;
    public float constantSpeed;

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
        GotClicked?.Invoke(correctOne);
        if (correctOne) {
            transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().color = Color.yellow;
        }
    }

}
