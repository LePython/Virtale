using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadingScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnFadeFinished;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetFadeState(bool stet)
    {
        animator.SetBool("FadeIn", stet);
        Invoke("InvokeOnFade", 1f);
    }
    private void InvokeOnFade()
    {
        OnFadeFinished.Invoke();
    }
}
