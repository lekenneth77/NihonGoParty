using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KCRotatePlayer : MonoBehaviour
{
    // Start is called before the first frame update
    public RotateObject sphere;
    public GameObject[] rotatingBoxes;
    public TextMeshProUGUI centerText;
    public GameObject[] stars;

    public float currentRotation;
    public int wins;


    private Vector3[] confirmPositions = new Vector3[] { new Vector3(0, 0.5f, 0),
                            new Vector3(-0.5f, 0, 0), new Vector3(0, -0.5f, 0), new Vector3(0.5f, 0, 0)};


    public void RotateSphere(float direction)
    {
        currentRotation = Mathf.Abs(currentRotation + direction) >= 360 ? 0 : currentRotation + direction;
        sphere.Rotate(direction, "z");
        foreach (GameObject rotators in rotatingBoxes)
        {
            GameObject textChild = rotators.transform.GetChild(0).gameObject;
            float currentZ = textChild.transform.rotation.eulerAngles.z;
            textChild.transform.rotation = Quaternion.Euler(0, 0, currentZ - direction);
        }
    }

    public bool Win()
    {
        stars[wins].SetActive(true);
        wins++;
        return wins == 3;
    }

    public void ChangeSpeed(float speed)
    {
        foreach (GameObject rotator in rotatingBoxes)
        {
            rotator.GetComponent<MoveObject>().ChangeSpeed(speed);
        }
    }

    public void ConfirmBoxes()
    {
        for (int i = 0; i < rotatingBoxes.Length; i++)
        {
            rotatingBoxes[i].GetComponent<MoveObject>().SetTargetAndMove(confirmPositions[i]);
        }
    }

    public void GoHomeBoxes()
    {
        foreach (GameObject rotator in rotatingBoxes)
        {
            rotator.GetComponent<MoveObject>().GoHome();
        }
    }

    public void SetRotatingText(string text)
    {
        for (int i = 0; i < rotatingBoxes.Length; i++)
        {
            rotatingBoxes[i].transform.GetChild(0).gameObject.GetComponent<TextMeshProUGUI>().text = text[i] + "";
        }
    }
}
