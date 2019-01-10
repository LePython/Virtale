using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AudioManager;
using UnityEngine.ML;

public class EnvironmentAssigner : MonoBehaviour
{
    private enum environment
    {
        G0 = 0,
        G1 = 1,
        G2 = 2,
        G3 = 3,
        G4 = 4,
        G5 = 5,
        G6 = 6,
        G7 = 7,
        G8 = 8,
        G9 = 9,
    }

    [SerializeField]
    private environment currentEnvironment;

    // AudioManager to get musicplaylist from
    [SerializeField]
    private AudioPlaybackManager audioManager;

    // Feature manager to get already analyzed song list
    [SerializeField]
    private FeatureAnalysisManager featureManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void compareSongLists()
    {
        string currentSongName = audioManager.MusicList.SongPlaylist[AudioPlaybackManager.SongNumber].songName;
        SongData result = featureManager.AnalyzedSongList.Find(x => x.Name == currentSongName + ".wav");
        Debug.Log( $"Song is already analyzed. Song title: {currentSongName}, song feature group: {result.FeatureGroup}" );
        currentEnvironment = (environment)result.FeatureGroup;
    }
}
