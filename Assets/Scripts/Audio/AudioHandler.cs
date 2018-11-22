using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace UnityEngine.AudioManager
{
    /// <summary>
    /// Audio handler handles how audio is loaded, where
    /// it is loaded from, saves the audio data and 
    /// contains main components for audio playback (eg. AudioSource)
    /// </summary>
    [System.Serializable]
    public class AudioHandler : MonoBehaviour
    {


        #region Unity Events

        public UnityEvent OnAudioSelected;

        #endregion

        #region Private Variables
        private List<string> validExtensions = new List<string> { "*.mp3", "*.wav" };

        // Main Audio Source attached to the component
        private AudioSource audioSource;
        // Master audio mixer
        [SerializeField]
        private AudioMixer masterMixer;

        // Current audio attached to the audio source component
        private static AudioClip audioToPlay;

        #endregion

        #region Unity Methods
        // Used before Start for initialization
        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
            if(!audioSource)
            {
                audioSource = gameObject.AddComponent<AudioSource>();
                return;
            }
            if (!masterMixer)
            {
                Debug.LogError("There is no AudioMixer attached to Master Mixer");
            }
        }
        private void Start()
        {
            
        }
        #endregion

        #region Properties
        // Getter for the audio source
        public AudioSource GetAudioSource
        {
            get
            {
                return audioSource;
            }
        }
        // Master Volume of the Audio Mixer
        [SerializeField]
        private float MasterVolume
        {
            get
            {
                float volume = 0;
                if (masterMixer.GetFloat("masterVolume", out volume))
                {
                    return volume;
                }
                else
                {
                    return 0f;
                }
            }
        }

        public static AudioClip AudioToPlay
        {
            get
            {
                return audioToPlay;
            }
            set
            {
                audioToPlay = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Stream audio file
        /// </summary>
        /// <param name="playlist"> Playlist to stream from </param>
        /// <param name="index"> Audio file index in playlist to stream </param>
        public void StreamAudio(MusicPlaylist playlist, int index)
        {
            StartCoroutine(LoadAudio(playlist.Playlist[index]));
        }
        /// <summary>
        /// Stream audio file
        /// </summary>
        /// <param name="path"> Enter audio file path </param>
        public void StreamAudio(string path)
        {
            StartCoroutine(LoadAudio("file://" + Application.streamingAssetsPath + "/" + path));
        }
        private IEnumerator LoadAudio(string audioFilePath)
        {
            if(audioFilePath == null)
                yield break;
            WWW request = GetAudioFromFile(audioFilePath);
            
            yield return request;

            audioToPlay = request.GetAudioClip(false, true, AudioType.WAV);
            Debug.Log(audioToPlay.name + " loaded");
            audioSource.clip = audioToPlay;

            // Invoke selected method when the new song is selected
            OnAudioSelected.Invoke();
        }
        private WWW GetAudioFromFile(string path)
        {
            WWW request = new WWW(path);
            return request;
        }
        #endregion

    }

}
