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
        finishPosition = new Vector3(-38f, -0.5f, 0);
    }

    public void Run() {
        Debug.Log("hello");
        runner.ChangeSpeed(runSpeed);
        runner.SetTargetAndMove(finishPosition);
        finish = false;
    }

    void Update() { 
        if (!finish && !runner.GetMoveFlag()) {
            finish = true;
            ReachedEnd?.Invoke();
        }
    }

}
