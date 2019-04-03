using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;
using TMPro;
using UnityEngine.Events;

/// <summary>
/// Transcript structure is going to be used to manage audio, displayed text and delay each time
/// VisualAssistant begins another sentence
/// </summary>
[System.Serializable]
public struct Transcript
{
    public string transcriptPath;
    public string transcriptText;
    public float delayBefore;

    public Transcript(string audioPath, string text, float delay)
    {
        this.transcriptPath = audioPath;
        this.transcriptText = text;
        this.delayBefore = delay;
    }
    public Transcript(string audioPath, string text)
    {
        this.transcriptPath = audioPath;
        this.transcriptText = text;
        this.delayBefore = 0;
    }
}

public class SpeechSequence : MonoBehaviour
{

    [SerializeField]
    private UnityEvent OnVisualAssistantEnd;
    
    [SerializeField]
    private AudioHandler audioHandler;

    [SerializeField]
    private List<Transcript> transcriptList;

    [SerializeField]
    private TextMeshProUGUI transcriptText;

    private int transcriptListIndex = 0;

    private void Awake() {
        if(!audioHandler)
        {
            audioHandler = gameObject.GetComponent<AudioHandler>();
            if(audioHandler == null)
                Debug.Log("There is no audio Handler attached to the speech sequence GameObject or script. Disabling script");
                enabled = false;
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("WaitForTransitionToApp");
        transcriptList = new List<Transcript>();
        transcriptList.Add(new Transcript("Vassist/transcript_1.wav", "Willkommen in VirtaleVR", 2.0f));
        transcriptList.Add(new Transcript("Vassist/transcript_2.wav", "Um anzufangen, strecken Sie Ihre linke Hand vor Ihrem Gesicht aus", 2.0f));
        transcriptList.Add(new Transcript("Vassist/transcript_3.wav", "Um ein Lied abzuspielen, drücken sie im Wiedergabefenster auf den Wiedergabeknopf", 3.0f));
        audioHandler.StreamAudio(transcriptList[transcriptListIndex].transcriptPath);
        transcriptText.text = transcriptList[transcriptListIndex].transcriptText;
    }

    private IEnumerator WaitForTransitionToApp()
    {
        while(AudioPlaybackManager.musicPlaybackState == AudioPlaybackManager.PlaybackState.Pause)
        {
            yield return null;
        }

        OnVisualAssistantEnd.Invoke();

        yield break;
    }
    /// <summary>
    /// The next Transcript gets prepared for streaming by invoking a method from UnityEvents in Inspector
    /// </summary>
    public void PlayTranscriptList()
    {
        transcriptListIndex++;

        if(transcriptListIndex <= transcriptList.Count-1)
        {
            Invoke("PrepareTranscript", transcriptList[transcriptListIndex].delayBefore + audioHandler.GetAudioSource.clip.length);
        }
    }

    private void PrepareTranscript()
    {
        audioHandler.StreamAudio(transcriptList[transcriptListIndex].transcriptPath);
        transcriptText.text = transcriptList[transcriptListIndex].transcriptText;
        //StartCoroutine(TextOpacityTransition(transcriptText, true));
    }

    private IEnumerator TextOpacityTransition(TextMeshProUGUI text, bool appear)
    {
        bool textAppear = appear;
        float a = text.color.a;
        while(a > 0)
        {
            a -= 0.01f;
            text.color = new Color(255,255,255,a);
            yield return null;
        }
    }
}
