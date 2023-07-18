using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleBasket : MonoBehaviour
{
    public event Action<string> gotApple;
    private void OnTriggerEnter(Collider other)
    {
        gotApple?.Invoke(other.gameObject.GetComponent<Apple>().text);
        Destroy(other.gameObject);
    }
}
