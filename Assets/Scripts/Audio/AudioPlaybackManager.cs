using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace UnityEngine.AudioManager
{
    [RequireComponent(typeof(AudioHandler)), System.Serializable]
    internal class AudioPlaybackManager : MonoBehaviour, IMusicPlayer
    {

        public static AudioPlaybackManager Instance;

        // Playback states enumerator
        public enum PlaybackState { Play, Pause };

        #region Events

        public UnityEvent OnPlaybackStateChanged;

        public UnityEvent OnSongEnd;

        #endregion

        #region Serialized Variables

        [SerializeField]
        private MusicPlaylist musicList;
        #endregion
        #region Public Variables

        public static PlaybackState musicPlaybackState = PlaybackState.Pause;

        #endregion

        #region Private Variables

        private AudioHandler audioHandler;
        private float _audioVolume = 0.0f;

        private static int songNumber = 0;

        // Time when the music is paused is saved in this variable
        private static float musicPausedTime;

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
        IEnumerator WaitForSongEnd()
        {
            while(audioHandler.GetAudioSource.time < audioHandler.GetAudioSource.clip.length)
            {
                yield return null;
            }
            OnSongEnd.Invoke();
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
            SelectSong(songNumber);
        }

        // Update is called once per frame
        void Update()
        {
            //int min = Mathf.FloorToInt(audioHandler.GetAudioSource.time / 60);
            //int sec = Mathf.FloorToInt(audioHandler.GetAudioSource.time % 60);
            ////Debug.Log(min + "m:" + sec +"s");
            // if ((int)AudioHandler.GetAudioSource.time == (int)AudioHandler.GetAudioSource.clip.length)
            // {
            //     musicPlaybackState = PlaybackState.Pause;
            //     NextSong();
            // }
            ManageInput();
        }

        #endregion

        #region Privately Defined Methods

        public void SelectSong(int index)
        {
            musicPlaybackState = PlaybackState.Pause;
            audioHandler.StreamAudio(musicList, index);
            StartCoroutine(WaitForSongEnd());
        }
        // Set the Audio Volume to 0. if Audio volume is 0,
        // it pauses automatically
        public void PauseAudio()
        {
            musicPlaybackState = PlaybackState.Pause;
            AudioVolume        = 0f;   // Song pauses itself if audio volume is 0
            MusicPausedTime    = Time.time;
        }
        // Set the volume to 1 and start playing the music
        public void PlayAudio()
        {
            musicPlaybackState = PlaybackState.Play;
            AudioVolume        = 1f;
            audioHandler.GetAudioSource.Play();
        }
        public void NextSong()
        {
            StopCoroutine(WaitForSongEnd());
            Instance.SelectSong(++songNumber);
        }
        public void LastSong()
        {
            StopCoroutine(WaitForSongEnd());
            Instance.SelectSong(--songNumber);
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

        public static float MusicPausedTime
        {
            get
            {
                return musicPausedTime;
            }

            set
            {
                musicPausedTime = value;
            }
        }

        public MusicPlaylist MusicList { get => musicList; }
        public static int SongNumber { get => songNumber; set => songNumber = value; }


        #endregion

        #region Event Handlers
        /// <summary>
        /// If Bumper button is pressed, change playback state
        /// to either play or pause.
        /// </summary>
        /// <param name="controller_id"></param>
        /// <param name="buttonPressed"></param>
        private void ManageInput()
        {
            if(Input.GetKeyDown(KeyCode.P))
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
            // Home button to select the next song in the playlist
            if (Input.GetKeyDown(KeyCode.N))
            {
                if(songNumber >= musicList.SongPlaylist.Count)
                {
                    songNumber = 0;
                }
                NextSong();
            }
            OnPlaybackStateChanged.Invoke();
        }
        #endregion
    }


}
