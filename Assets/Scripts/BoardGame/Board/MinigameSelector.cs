using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MinigameSelector : MonoBehaviour
{
    public GameObject leaderboard;
    public TextMeshProUGUI categoryText;
    public GameObject[] threeMinigames;
    private static List<int> chosen;
    public static event Action<int> gotGame;

    private float[] delayMultipliers = new float[] { 1f, 1.1f, 1.3f };

    // Start is called before the first frame update
    void Start()
    {
    }

     public void ChangeText(string[] names, string category) {
        chosen = new List<int>();
        for (int i = 0; i < 3; i++) {
            int random = UnityEngine.Random.Range(0, names.Length);
            while (chosen.Contains(random)) { 
                random = UnityEngine.Random.Range(0, names.Length);
            }
            chosen.Add(random);
            threeMinigames[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = names[random];            
        }
        categoryText.text = category;
    }

    private void OnEnable() {
        leaderboard.SetActive(false);
        StartCoroutine("ChooseGame");
    }
   

    private IEnumerator ChooseGame() {
        yield return new WaitForSeconds(1f);
        float delayTime = 0.025f;
        int index = 0;
        float maxTime = UnityEngine.Random.Range(.4f, .6f);
        while (delayTime < maxTime) {
            GameObject prev = index == 0 ? threeMinigames[2] : threeMinigames[index - 1];
            prev.transform.localScale = Vector3.one;
            threeMinigames[index].transform.localScale = new Vector3(1.2f, 1.3f, 1f);

            int random = UnityEngine.Random.Range(0, delayMultipliers.Length);
            delayTime *= delayMultipliers[random];
            yield return new WaitForSeconds(delayTime);
            if (delayTime < maxTime) { 
                index = index + 1 >= 3 ? 0 : index + 1;
            }
        }
        threeMinigames[index].GetComponent<Animator>().enabled = true;
        threeMinigames[index].GetComponent<Animator>().Play("ZoomInOut");
        yield return new WaitForSeconds(5f);
        gotGame?.Invoke(chosen[index]);
        leaderboard.SetActive(true);
        threeMinigames[index].GetComponent<Animator>().enabled = false;
        threeMinigames[index].transform.localScale = Vector3.one;
        gameObject.SetActive(false);
    }
   
}
