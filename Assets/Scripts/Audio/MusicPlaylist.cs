using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace UnityEngine.AudioManager
{
    [System.Serializable]
    public class MusicPlaylist : MonoBehaviour
    {

        #region Serialized Variables
        [SerializeField]
        private string audioPlaylistPath;

        [SerializeField]
        private List<string> audioPathList = new List<string>();
        #endregion

        #region Properties
        public List<string> Playlist { get => audioPathList; set => audioPathList = value; }
        #endregion

        #region Unity Methods

        private void Awake() {
            if(audioPlaylistPath != null)
            {
                CreateListFromPath(audioPlaylistPath);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Gets the path of the folder or so called playlist, because playlists are going 
        /// to be stored in folders. It then automatically assignes all found .wav files to
        /// the initialized list.
        /// </summary>
        /// <param name="folder"> Give only the name of the folder. No slashes </param>
        public void CreateListFromPath(string folder)
        {
            if(audioPathList.Count > 0)
                Debug.LogWarning("You are creating a list on top of another list. If this was not your intention, you should remove list path in your inspector window");

            string audioPath = Application.streamingAssetsPath + "/"+ folder;
            DirectoryInfo directoryInfo = new DirectoryInfo(audioPath);

#if UNITY_EDITOR
            Debug.Log("Streaming Assets Path:" + audioPath);
#endif

            FileInfo[] allFiles = directoryInfo.GetFiles("*.wav", SearchOption.AllDirectories);

#if UNITY_EDITOR
            Debug.Log(allFiles.Length + " files will be loaded into the list.");
#endif
            audioPathList = new List<string>();
            foreach(FileInfo file in allFiles)
            {
                if (file.Extension == ".wav")
                {
                    audioPathList.Add("file://" + file.FullName);
                }
            }
        }

        #endregion


    }
}