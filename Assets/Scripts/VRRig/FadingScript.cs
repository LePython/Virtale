using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class FadingScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnFadeInFinished;
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }
    private void Start()
    {
        SetFadeState(false);
    }
    // If you want to fade in, set the bool to true. False otherwise.
    public void SetFadeState(bool fadein)
    {
        animator.SetBool("FadeIn", fadein);
        if(fadein == true)
            Invoke("InvokeOnFadeInEnd", 1f);
    }
    private void InvokeOnFadeInEnd()
    {
        OnFadeInFinished.Invoke();
    }
    public void LoadScene(int sceneNumber)
    {
        StartCoroutine(LoadSceneAsync(sceneNumber));
    }
    private IEnumerator LoadSceneAsync(int sceneIndex)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);

        while(!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
