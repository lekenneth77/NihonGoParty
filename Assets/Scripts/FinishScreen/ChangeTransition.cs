using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ChangeTransition : MonoBehaviour
{
    public CinemachineBrain brain;
    
    public void ChangeSpeed(float speed)
    {
        brain.m_DefaultBlend.m_Time = speed;
    }
    
}
