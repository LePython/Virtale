using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;

public class AnimatePlayPauseButton : MonoBehaviour
{

    private Animator playbackButtonAnimator;

    private void Awake() 
    {
        playbackButtonAnimator = GetComponent<Animator>();
    }

    public void CheckState()
    {
        if(AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Play)
        {
            playbackButtonAnimator?.SetBool("IsPlaying", true);
        }
        else
        {
            playbackButtonAnimator?.SetBool("IsPlaying", false);
        }
    }
}
