using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class KCSolutionPopup : MonoBehaviour
{
    public TextMeshProUGUI[] sideBoxes;
    public TextMeshProUGUI centerBox;

    public TextMeshProUGUI[] textSolutions;

    public void UpdateSolutionBoard(string center, string sides)
    {
        centerBox.text = center;
        for (int i = 0; i < sideBoxes.Length; i++)
        {
            sideBoxes[i].text = sides[i] + "";
        }

        textSolutions[0].text = sides[0] + center;
        textSolutions[1].text = sides[1] + center;
        textSolutions[2].text = center + sides[2] ;
        textSolutions[3].text = center + sides[3] ;
    }
}
