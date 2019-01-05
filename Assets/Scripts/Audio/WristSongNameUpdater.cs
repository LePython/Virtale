using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AudioManager;
public class WristSongNameUpdater : MonoBehaviour
{
    [SerializeField]
    private AudioPlaybackManager audioManager;

    private TextMeshProUGUI textMesh;

    private void Awake() 
    {
        textMesh = GetComponent<TextMeshProUGUI>();
        if(!textMesh)
        {
            Debug.LogError("Attach the text mesh first on wrist song displayer");
            enabled = false;
        }
        if(!audioManager)
        {
            textMesh.text = "There is no audio manager assigned.";
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        textMesh.text = audioManager.MusicList.SongPlaylist[AudioPlaybackManager.SongNumber].songName;
    }

    public void UpdateTextMesh()
    {
        if(gameObject.activeSelf)
        {
            textMesh.text = audioManager.MusicList.SongPlaylist[AudioPlaybackManager.SongNumber].songName;
        }
    }
}
