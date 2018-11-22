using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;

/// <summary>
/// Transcript structure is going to be used to manage audio, displayed text and delay each time
/// VisualAssistant begins another sentence
/// </summary>
[System.Serializable]
public struct Transcript
{
    private string transcriptPath;
    private string transcriptText;
    private float delayBefore;

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

    // List of audio files for speech sequencing
    [SerializeField]
    private MusicPlaylist speechSequenceList;

    [SerializeField]
    private AudioHandler audioHandler;

    [SerializeField]
    private List<Transcript> transcriptList;

    private void Awake() {
        if(!audioHandler)
        {
            audioHandler = gameObject.GetComponent<AudioHandler>();
            if(audioHandler == null)
                Debug.Log("There is no audio Handler attached to the speech sequence GameObject or script. Disabling script");
                enabled = false;
            return;
        }
        if(!speechSequenceList)
        {
            speechSequenceList = gameObject.AddComponent<MusicPlaylist>();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        transcriptList = new List<Transcript>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
