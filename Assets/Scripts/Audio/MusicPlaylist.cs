using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
        #endregion

        #region Properties
        public List<Song> SongPlaylist { get => songPlaylist; set => songPlaylist = value; }
        #endregion

        #region Unity Methods

        private void Awake() {
            if(audioPlaylistPath != null)
            {
                CreateListFromPath(audioPlaylistPath, audioExtensions);
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
        #endregion

    }
}