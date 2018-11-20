using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;

public class MusicBoxManager : MonoBehaviour {


    #region Private Variables

    // One-time use variables for popping sound
    private float popSoundDelay;

    // Fetch the Music Box Animator
    private Animator musicBoxAnimator;

    #endregion

    #region Unity Methods
    // Use this for initialization
    void Start ()
    {

        // Set pop sound delay
        popSoundDelay = 1.0f - transform.GetComponent<AudioSource>().clip.length;

        musicBoxAnimator = GetComponent<Animator>();

        StartCoroutine(PlayPopSound(popSoundDelay));
        
    }
    #endregion

    #region Private Methods
    
    //Play pop sound with defined delay
    IEnumerator PlayPopSound(float delay)
    {
        yield return new WaitForSeconds(delay);
        transform.GetComponent<AudioSource>().Play();
    }
    /// <summary>
    /// Check the state the music player is in and then change the visual music box state accordingly
    /// This method is being invoked by the AudioPlaybackManager class
    /// </summary>
    public void ChangeVisualPlaybackState()
    {
        AudioPlaybackManager.PlaybackState state = AudioPlaybackManager.musicPlaybackState;
        if (state == AudioPlaybackManager.PlaybackState.Pause)
        {
            musicBoxAnimator.SetBool("IsPlaying", false);
        }
        else if (state == AudioPlaybackManager.PlaybackState.Play)
        {
            musicBoxAnimator.SetBool("IsPlaying", true);
        }
    }

    #endregion
}
