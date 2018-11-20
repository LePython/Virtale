using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;
public class SpeechSequence : MonoBehaviour
{
    // List of audio files for speech sequencing
    [SerializeField]
    private MusicPlaylist speechSequence;

    private void Awake() {
        if(!speechSequence)
        {
            speechSequence = gameObject.AddComponent<MusicPlaylist>();
            return;
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
