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

        // Current audio attached to the audio source component
        private static AudioClip audioToPlay;

        private float audioLength;

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
        }
        private void Start()
        {
            //StreamAudio("Tutorial/Tony Anderson - Immanuel.wav");
            StreamHTTPAudio("https://virtalevr.federalchat.eu/getAudio/Royalty Free Music - Electronic Fantasy.wav");
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

        public float AudioLength { get => audioLength; }

        #endregion

        #region Private Methods

        /// <summary>
        /// Stream audio file from a list
        /// </summary>
        /// <param name="playlist"> Playlist to stream from </param>
        /// <param name="index"> Audio file index in playlist to stream </param>
        public void StreamAudio(MusicPlaylist playlist, int index)
        {
            StartCoroutine(LoadAudio(playlist.SongPlaylist[index].songPath));
        }
        /// <summary>
        /// Stream audio file
        /// </summary>
        /// <param name="path"> Enter audio file path </param>
        public void StreamAudio(string path)
        {
            StartCoroutine(LoadAudio("file://" + Application.streamingAssetsPath + "/" + path));
        }
        /// <summary>
        /// Stream audio from the given website url.
        /// </summary>
        /// <param name="url"></param>
        public void StreamHTTPAudio(string url)
        {
            StartCoroutine(LoadAudio(url));
        }
        
        private IEnumerator LoadAudio(string audioFilePath)
        {
            AudioToPlay = null;
            Resources.UnloadUnusedAssets();  // YOU ARE FUCKING KIODDNGF ME?!!!! BECAUSE OF THIS SHIT I SPENT HOURS WHAT THE FUCK IS WRONG WITH MY CODE

            if(audioFilePath == null)
                yield break;

            UnityWebRequest request = GetAudioFromFile(audioFilePath);
            request.disposeCertificateHandlerOnDispose = true;
            request.disposeDownloadHandlerOnDispose = true;
            request.disposeUploadHandlerOnDispose = true;

            yield return request.SendWebRequest();

            audioToPlay = DownloadHandlerAudioClip.GetContent(request);
            
            audioLength = AudioToPlay.length;
            audioSource.clip = audioToPlay;

            // Invoke selected method when the new song is selected
            OnAudioSelected.Invoke();

            request.Dispose();

            yield break;
        }
        private UnityWebRequest GetAudioFromFile(string path)
        {
            UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(path, AudioType.WAV);
            return request;
        }

        #endregion

    }

}
