using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpObject : MonoBehaviour
{
    public float duration = 2f;
    public float height = 3f;

    private Vector3 starting;
    private Vector3 target;
    private bool triggerJump;
    private float anim;

    private void Start()
    {
        triggerJump = false;
    }

    public void SetupAndJump(Vector3 tgt, float d, float h) {
        starting = transform.position;
        target = tgt;
        duration = d;
        height = h;
        
        triggerJump = true;
    }

    private bool GetJump() {
        return triggerJump;
    }

    // Update is called once per frame
    void Update()
    {
        if (!triggerJump) { return; }

        Jump();
    }

    private void Jump() {
        anim += Time.deltaTime;
        transform.position = MathParabola.Parabola(starting, target, height, anim / duration);

        if (anim > duration)
        {
            transform.position = MathParabola.Parabola(starting, target, height, 1);
            triggerJump = false;
            anim = 0;
        }
    }
}
