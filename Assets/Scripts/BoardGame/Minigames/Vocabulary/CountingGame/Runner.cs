using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Runner : MonoBehaviour
{
    public MoveObject runner;
    public TextMeshProUGUI text;

    public float runSpeed;
    public Vector3 position;
    private bool finish;
    public static event Action ReachedEnd;

    private Vector3 finishPosition;
    void Start() {
        finish = true;
        finishPosition = new Vector3(-45f, -0.5f, 0);
    }

    public void Run() {
        runner.ChangeSpeed(runSpeed);
        runner.SetTargetAndMove(finishPosition);
        finish = false;
    }

    public void ChangeText(string s) {
        if (s.Length == 1) {
            text.fontSize = 1;
        } else if (s.Length == 2) {
            text.fontSize = 0.75f;
        } else if (s.Length == 3) {
            text.fontSize = 0.45f;
        } else {
            text.fontSize = 0.3f;
        }
        text.text = s;
    }

    void Update() { 
        if (!finish && !runner.GetMoveFlag()) {
            finish = true;
            ReachedEnd?.Invoke();
        }
    }


}
