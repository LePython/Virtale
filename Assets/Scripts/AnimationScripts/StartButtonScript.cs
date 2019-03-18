using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButtonScript : MonoBehaviour
{

    private Animator startPanelAnimator;  


    private void Awake() 
    {
        startPanelAnimator = GetComponent<Animator>();
    }

    public void StartPanelAnim()
    {
        startPanelAnimator?.SetBool("IsPressed", true);
    }
}
