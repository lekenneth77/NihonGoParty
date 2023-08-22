using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffsetScrolling : MonoBehaviour {
    public float scrollSpeed = .2f;
    public bool horizontal;
    private MeshRenderer rend;
    

    void Start () {
        rend = GetComponent<MeshRenderer>();
    }

    void Update () {
        if (horizontal) {
            rend.material.mainTextureOffset = new Vector2((Time.time * scrollSpeed), 0f);
        } else { 
            rend.material.mainTextureOffset = new Vector2(-(Time.time * scrollSpeed) / 2f, (Time.time * scrollSpeed) / 2f);
        }
    }
}
