using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Image[] startingScreens;
    public Image fadeoutScreen;
    public static bool skipStart; //eventually use this so that you don't have to watch the beginning screens

    // Start is called before the first frame update
    void Start()
    {
        if (skipStart) {
            fadeoutScreen.gameObject.SetActive(false);
        } else {
            StartCoroutine("DoScreens");
        }
    }

    public IEnumerator DoScreens() {
        yield return new WaitForSeconds(2f);
        foreach (Image img in startingScreens) {
            img.gameObject.SetActive(true);
            for (float i = 0f; i <= 1f; i += 0.01f) { 
                Color col = img.color;
                col.a = i;
                img.color = col;
                yield return new WaitForSeconds(0.01f);
            }

            yield return new WaitForSeconds(1f);
            for (float i = 1f; i >= 0f; i -= 0.01f) { 
                Color col = img.color;
                col.a = i;
                img.color = col;
                yield return new WaitForSeconds(0.01f);
            }
            img.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        fadeoutScreen.gameObject.SetActive(true);
        for (float i = 1f; i >= 0f; i -= 0.01f) { 
                Color col = fadeoutScreen.color;
                col.a = i;
                fadeoutScreen.color = col;
                yield return new WaitForSeconds(0.015f);
        }
        fadeoutScreen.gameObject.SetActive(false);
    }

    public void LoadCredits() {
        //Color col = fadeoutScreen.color;
        //col.a = 0f;
        //fadeoutScreen.color = col;

        //fadeoutScreen.gameObject.SetActive(true);
        //while (fadeoutScreen.color.a < 1f)
        //{
        //    Color tmp = fadeoutScreen.color;
        //    float fadeAmt = tmp.a + (1 * Time.deltaTime);
        //    tmp.a = fadeAmt;
        //    fadeoutScreen.color = tmp;
        //    yield return null;
        //}
        skipStart = true;
        SceneManager.LoadSceneAsync("Credits");
    }


    public void StartPlay() {
        skipStart = true;
        SceneManager.LoadSceneAsync("Multiplayer Setup");
    }

}
