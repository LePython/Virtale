using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SoundDelay : MonoBehaviour
{
    [SerializeField]
    private UnityEvent OnPlayEnd;

    [SerializeField]
    private float delay;
    private WaitForSeconds waitDelay;
    private AudioSource audioSource;
    private IEnumerator delayPlay()
    {
        yield return waitDelay;
        audioSource.Play();
        
        while((audioSource.clip.length + delay) > Time.time)
        {
            yield return null;
        }
        OnPlayEnd.Invoke();
        
        yield break;
    }
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if(!audioSource)
        {
            enabled = false;
        }
        audioSource.playOnAwake = false;
        waitDelay = new WaitForSeconds(delay);
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayPlay());   
    }
}
