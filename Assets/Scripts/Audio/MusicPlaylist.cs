using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.AudioManager;
using Newtonsoft.Json;

namespace UnityEngine.AudioManager
{
    [Serializable]
    public class Song
    {
        public string songName;
        public string songPath;

        public Song(string name, string path)
        {
            songName = name;
            songPath = path;
        }
    }

    [System.Serializable]
    public class MusicPlaylist : MonoBehaviour
    {

        #region Serialized Variables
        [SerializeField]
        private string audioPlaylistPath;

        [SerializeField]
        private List<Song> songPlaylist = new List<Song>();

        private string[] audioExtensions = {".mp3",".wav"};

        private ServerHandler srvHandler;
        #endregion

        #region Properties
        public List<Song> SongPlaylist { get => songPlaylist; set => songPlaylist = value; }
        #endregion

        #region Unity Methods

        private void Awake() 
        {
            srvHandler = gameObject.AddComponent<ServerHandler>();
            if(audioPlaylistPath != null)
            {
                CreateListFromPath(audioPlaylistPath, audioExtensions);
            }
        }

        private void Start() 
        {
            ServerHandler.AudioAnalyze aud = new ServerHandler.AudioAnalyze{url = "https://www.youtube.com/watch?v=LYmkWqI0RLg", format = "wav", key = "M2qetha1abruFeb@itIsastuy1weprIpr9k545goprl@5kiVaCRez4ThAQL5pa4p"};
            string json = JsonConvert.SerializeObject(aud);

            // Set the delegate to the function I want to call after the GET request has ended
            srvHandler.OnRequestEnd += srvHandler.GetAudioList;
            srvHandler.OnRequestEnd += UpdateMusicList;

            StartCoroutine(srvHandler.GetRequest("https://virtalevr.federalchat.eu/getSongList", srvHandler.OnRequestEnd));

        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the path of the folder or so called playlist, because playlists are going 
        /// to be stored in folders. It then automatically assignes all found .wav files to
        /// the initialized list.
        /// </summary>
        /// <param name="folder"> Give only the name of the folder. No slashes </param>
        public void CreateListFromPath(string folder, string[] fileExtensions)
        {
            if(songPlaylist.Count > 0)
                Debug.LogWarning("You are creating a list on top of another list. If this was not your intention, you should remove list path in your inspector window");

            DirectoryInfo audioPath = new DirectoryInfo(Application.streamingAssetsPath + "/"+ folder);


            IEnumerable<FileInfo> audioFiles = AudioFileGetter.GetFilesByExtensions(audioPath, fileExtensions);

#if UNITY_EDITOR
            Debug.Log("Streaming Assets Path:" + audioPath);
            //Debug.Log(audioFiles.Length + " files will be loaded into the list.");
#endif
            foreach(FileInfo file in audioFiles)
            {
                songPlaylist.Add(new Song(Path.GetFileNameWithoutExtension(file.Name), "file://" + file.FullName));
            }
        }

        /// <summary>
        /// Automatically waits until the requested list is loaded in ServerHandler.
        /// </summary>
        /// <param name="jsonFileList"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public void CreateMusicListFromJSON<T>(List<T> jsonFileList)
        {
            FieldInfo[] fields = typeof(T).GetFields();
            foreach(FieldInfo field in fields)
            {
                if(field.Name == "name")
                {
                    Debug.Log(field.Name);
                }
            }
            //songPlaylist = srvHandler.RequestedAudioList;
        }

        /// <summary>
        /// Function to attach to the OnRequestEnd delegate in ServerHandler.
        /// </summary>
        private void UpdateMusicList()
        {
            CreateMusicListFromJSON<ServerHandler.AudioData>(srvHandler.RequestedAudioList);
        }
        #endregion
        
    }
}