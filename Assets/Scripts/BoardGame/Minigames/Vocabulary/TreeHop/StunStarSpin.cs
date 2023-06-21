using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StunStarSpin : MonoBehaviour
{
    public float speed = 1.5f;
    public float activeTime = 2f;

    public bool trigger = false;

    public void StartSpin() {
        gameObject.SetActive(true);
        trigger = true;
        StartCoroutine("EndSpin");
    }

    private IEnumerator EndSpin() {
        yield return new WaitForSeconds(activeTime);
        trigger = false;
        gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!trigger) { return; }
        gameObject.transform.Rotate(0, speed, 0);
    }
}
