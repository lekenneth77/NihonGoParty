using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CircularTimer : MonoBehaviour
{
    public Timer timer;

    // Update is called once per frame
    void Update()
    {
        float timePassed = Mathf.Clamp01(timer.CurrentTime() / timer.timeLimit);
        gameObject.GetComponent<Image>().fillAmount = timePassed;
    }
}
