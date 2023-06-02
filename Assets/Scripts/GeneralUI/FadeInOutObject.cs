using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FadeInOutObject : MonoBehaviour
{
    public float frameDelay = 0.01f;
    public float waitBeforeOut = 1f;
    public bool skipFadeIn;

    //this code is not hot
    public void InitiateFade()
    {
        if (skipFadeIn)
        {
            Color temp = GetColor();
            temp.a = 1f;
            SetColor(temp);
        }
        this.gameObject.SetActive(true);
        StartCoroutine("Fade");
    }

    private IEnumerator Fade()
    {
        if (!skipFadeIn)
        {
            for (int i = 0; i < 100; i++)
            {
                Color temp = GetColor(); //gotta set it to something apparently
                temp.a += 0.01f;
                SetColor(temp);
                yield return new WaitForSeconds(frameDelay);
            }
        }

        yield return new WaitForSeconds(waitBeforeOut);

        for (int i = 0; i < 100; i++)
        {
            Color temp = GetColor(); //gotta set it to something apparently
            temp.a -= 0.01f;
            SetColor(temp);
            yield return new WaitForSeconds(frameDelay);
        }
        this.gameObject.SetActive(false);

    }

    //oh god why
    private Color GetColor()
    {
        Color temp = Color.white;
        if (this.gameObject.GetComponent<Image>())
        {
            temp = this.gameObject.GetComponent<Image>().color;
        }
        else if (this.gameObject.GetComponent<SpriteRenderer>())
        {
            temp = this.gameObject.GetComponent<SpriteRenderer>().color;
        }
        else if (this.gameObject.GetComponent<TextMeshProUGUI>())
        {
            temp = this.gameObject.GetComponent<TextMeshProUGUI>().color;
        }
        return temp;
    }

    private void SetColor(Color temp)
    {
        if (this.gameObject.GetComponent<Image>())
        {
            this.gameObject.GetComponent<Image>().color = temp;
        }
        else if (this.gameObject.GetComponent<SpriteRenderer>())
        {
            this.gameObject.GetComponent<SpriteRenderer>().color = temp;
        }
        else if (this.gameObject.GetComponent<TextMeshProUGUI>())
        {
            this.gameObject.GetComponent<TextMeshProUGUI>().color = temp;
        }
    }

}
