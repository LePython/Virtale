using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadingScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnFadeFinished;
    private Animator animator;

    private IEnumerator WaitForFadeStateChange(float delay)
    {
        float currentTime = Time.time;

        while(Time.time - currentTime > delay)
        {
            yield return null;
        }

        OnFadeFinished.Invoke();

        yield break;
    }
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    public void SetFadeState(bool stet)
    {
        animator.SetBool("FadeIn", stet);
        StartCoroutine(WaitForFadeStateChange(1.0f));
    }
}
