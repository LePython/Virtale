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

    private AudioSource audioSource;
    private IEnumerator delayPlay()
    {
        yield return new WaitForSeconds(delay);
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
    }
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(delayPlay());   
    }
}
