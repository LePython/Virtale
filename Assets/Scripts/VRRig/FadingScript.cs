using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
public class FadingScript : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnFadeOutNextSongEvent;

    [SerializeField]
    private UnityEvent OnFadeOutLastSongEvent;

    private Animator animator;

    private bool fadeInProgress = false;


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
        if(fadeInProgress == false)
        {
            fadeInProgress = true;
            animator.SetBool("FadeIn", fadein);
            Invoke("SetFadeProgressToFalse", 1f);
            if(!fadein)
            {
                Invoke("DisableFade", 1f);
            }
            else
            {
                Invoke("EnableFade", 1f);
            }
        }
    }
    private void DisableFade()
    {
        gameObject.SetActive(false);
    }
    private void EnableFade()
    {
        gameObject.SetActive(true);
    }
    private void SetFadeProgressToFalse()
    {
        fadeInProgress = false;
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

        yield break;
    }


    public void FadeOutNextSong()
    {
        Invoke("FadeOutNextSongInv", 1f);
    }
    public void FadeOutLastSong()
    {
        Invoke("FadeOutLastSongInv", 1f);
    }
    private void FadeOutNextSongInv()
    {
        OnFadeOutNextSongEvent.Invoke();
    }
    private void FadeOutLastSongInv()
    {
        OnFadeOutLastSongEvent.Invoke();
    }
}
