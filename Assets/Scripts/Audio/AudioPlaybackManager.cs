﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using TMPro;

namespace UnityEngine.AudioManager
{
    [RequireComponent(typeof(AudioHandler)), System.Serializable]
    internal class AudioPlaybackManager : MonoBehaviour, IMusicPlayer
    {

        private AudioPlaybackManager Instance;

        // Playback states enumerator
        public enum PlaybackState { Play, Pause };

        #region Events
        public UnityEvent OnPlaybackStateChanged;

        public UnityEvent OnSongEnd;

        #endregion

        #region Serialized Variables

        [SerializeField]
        private MusicPlaylist musicList;

        [SerializeField]
        private TextMeshProUGUI currentTimeTextMesh;

        #endregion

        #region Public Variables

        public static PlaybackState musicPlaybackState = PlaybackState.Pause;

        #endregion

        #region Private Variables
        private AudioHandler audioHandler;
        private float _audioVolume = 0.0f;

        private static int songNumber = 0;


        #endregion

        #region Coroutines
        /// <summary>
        /// Gradually change the audio volume to the specified volume
        /// </summary>
        /// <param name="volume"> Volume to change to</param>
        /// <returns></returns>
        IEnumerator DecayAudio(float volume)
        { 
            bool IsVolumeBigger = (volume > _audioVolume) ? true : false;
            if (IsVolumeBigger)
            {
                while (volume > _audioVolume)
                {
                    _audioVolume += 2 * Time.deltaTime;
                    audioHandler.GetAudioSource.volume = _audioVolume;
                    yield return null;
                }
            }
            else
            {
                while (volume < _audioVolume)
                {
                    _audioVolume -= 2 * Time.deltaTime;
                    audioHandler.GetAudioSource.volume = _audioVolume;
                    yield return null;
                }
                if (volume == 0f)
                {
                    audioHandler.GetAudioSource.Pause();
                }
            }
        }
        WaitForSeconds wait2Secs = new WaitForSeconds(.2f);
        IEnumerator WaitForSongEnd()
        {
            // die Lieddauer in Minuten und Sekunden umwandeln
            int audioLength = (int)audioHandler.AudioLength;
            int minLength = Mathf.FloorToInt(audioLength / 60);
            int secLength = Mathf.FloorToInt(audioLength % 60);
            string secLengthS = secLength.ToString();
            if(secLength < 10)
            {
                secLengthS = "0" + secLength;
            }

            // Die gegenwärtige Liedzeit in Minuten und Sekunden zeigen
            while((int)audioHandler.GetAudioSource.time < audioLength)
            {
                int tiem = (int)audioHandler.GetAudioSource.time;
                int min = Mathf.FloorToInt(tiem / 60);
                int sec = Mathf.FloorToInt(tiem % 60);
                
                string secunds = (sec < 10) ? "0" + sec : sec.ToString();

                currentTimeTextMesh.text = min + ":" + secunds + "/" + minLength + ":" + secLengthS;

                yield return wait2Secs;
            }
            OnSongEnd.Invoke();
            yield break;
        }
        #endregion

        #region Unity Methods

        void Awake()
        {
            Instance = this;
            if(!musicList)
            {
                musicList = gameObject.AddComponent<MusicPlaylist>();
                return;
            }
        }

        // Use this for initialization
        void Start()
        {
            audioHandler = GetComponent<AudioHandler>();
            Instance.SelectSong(songNumber);
        }

        #endregion

        #region Privately Defined Methods

        public void SelectSong(int index)
        {
            StopCoroutine("WaitForSongEnd");
            musicPlaybackState = PlaybackState.Pause;
            audioHandler.StreamAudio(musicList, index);
        }
        // Set the Audio Volume to 0. if Audio volume is 0,
        // it pauses automatically
        public void PauseAudio()
        {
            musicPlaybackState = PlaybackState.Pause;
            AudioVolume        = 0f;   // Song pauses itself if audio volume is 0
            OnPlaybackStateChanged.Invoke();
        }
        // Set the volume to 1 and start playing the music
        public void PlayAudio()
        {
            musicPlaybackState = PlaybackState.Play;
            AudioVolume        = 1f;
            audioHandler.GetAudioSource.Play();
            OnPlaybackStateChanged.Invoke();
        }
        public void NextSong()
        {
            if(SongNumber<musicList.SongPlaylist.Count-1)
            {
                Instance.SelectSong(++songNumber);
                OnPlaybackStateChanged.Invoke();
            }
            else
            {
                SceneManager.LoadScene(0);
            }
        }
        public void LastSong()
        {
            if(SongNumber>0)
            {
                Instance.SelectSong(--songNumber);
                OnPlaybackStateChanged.Invoke();
            }
        }

        public void StartWaitingOnEnd()
        {
            StartCoroutine("WaitForSongEnd");
        }

        #endregion

        #region Properties
        /// <summary>
        /// Getter give audio volume in range 0.0f - 1.0f
        /// Setter calls a DecayAudio Coroutine to gradually
        /// set the audio to the specified value
        /// </summary>
        private float AudioVolume
        {
            get
            {
                return _audioVolume;
            }
            set
            {
                StartCoroutine(DecayAudio(value));
            }
        }

        public MusicPlaylist MusicList { get => musicList; }
        public static int SongNumber { get => songNumber; set => songNumber = value; }


        #endregion

        // Decide if pause or play the song 
        public void PauseEventDecision()
        {
            if (musicPlaybackState == PlaybackState.Pause)
            {
                PlayAudio();
            }
            else
            {
                PauseAudio();
            }
        }

    }
}
