using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreen : MonoBehaviour
{
    public Image[] startingScreens;
    public Image fadeoutScreen;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("DoScreens");
    }

    public IEnumerator DoScreens() {
        yield return new WaitForSeconds(2f);
        foreach (Image img in startingScreens) {
            img.gameObject.SetActive(true);
            for (float i = 0f; i <= 1f; i += 0.01f) { 
                Color col = img.color;
                col.a = i;
                img.color = col;
                yield return new WaitForSeconds(0.025f);
            }

            yield return new WaitForSeconds(2f);
            for (float i = 1f; i >= 0f; i -= 0.01f) { 
                Color col = img.color;
                col.a = i;
                img.color = col;
                yield return new WaitForSeconds(0.025f);
            }
            img.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        fadeoutScreen.gameObject.SetActive(true);
        for (float i = 1f; i >= 0f; i -= 0.01f) { 
                Color col = fadeoutScreen.color;
                col.a = i;
                fadeoutScreen.color = col;
                yield return new WaitForSeconds(0.025f);
        }
        fadeoutScreen.gameObject.SetActive(false);
    }

    public void LoadCredits() { 

    }

    public void StartPlay() { 

    }

}
