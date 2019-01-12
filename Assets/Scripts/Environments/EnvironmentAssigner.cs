using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Visualizers;
using UnityEngine.AudioManager;
using UnityEngine.ML;

public class EnvironmentAssigner : MonoBehaviour
{
    public enum environment
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
        Default = 10
    }

    public environment currentEnvironment;

    // AudioManager to get musicplaylist from
    [SerializeField]
    private AudioPlaybackManager audioManager;

    // Feature manager to get already analyzed song list
    [SerializeField]
    private FeatureAnalysisManager featureManager;

    private bool isSpectrumUpdating = false;

    [ConditionalField("currentEnvironment", environment.G0), SerializeField]
    private SpectrumVisualizer G0Visualizer;
    [ConditionalField("currentEnvironment", environment.G1), SerializeField]
    private SpectrumVisualizer G1Visualizer;
    [ConditionalField("currentEnvironment", environment.G2), SerializeField]
    private SpectrumVisualizer G2Visualizer;
    [ConditionalField("currentEnvironment", environment.G3), SerializeField]
    private SpectrumVisualizer G3Visualizer;
    [ConditionalField("currentEnvironment", environment.G4), SerializeField]
    private SpectrumVisualizer G4Visualizer;
    [ConditionalField("currentEnvironment", environment.G5), SerializeField]
    private SpectrumVisualizer G5Visualizer;
    [ConditionalField("currentEnvironment", environment.G6), SerializeField]
    private SpectrumVisualizer G6Visualizer;
    [ConditionalField("currentEnvironment", environment.G7), SerializeField]
    private SpectrumVisualizer G7Visualizer;
    [ConditionalField("currentEnvironment", environment.G8), SerializeField]
    private SpectrumVisualizer G8Visualizer;
    [ConditionalField("currentEnvironment", environment.G9), SerializeField]
    private SpectrumVisualizer G9Visualizer;

    // Start is called before the first frame update
    void Start()
    {
        
    }
    // Checks if song is playing, if so, then starts a coroutine which updates spectrum
    public void CheckToUpdateVisualizer()
    {
        if(UnityEngine.AudioManager.AudioPlaybackManager.musicPlaybackState == UnityEngine.AudioManager.AudioPlaybackManager.PlaybackState.Play)
        {
            if(!isSpectrumUpdating)
            {
                isSpectrumUpdating = true;
                switch(currentEnvironment)
                {
                    case environment.G0:
                        G0Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G1:
                        G1Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G2:
                        G2Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G3:
                        G3Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G4:
                        G4Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G5:
                        G5Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G6:
                        G6Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G7:
                        G7Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G8:
                        G8Visualizer.StartSpectrumUpdating();
                        break;
                    case environment.G9:
                        G9Visualizer.StartSpectrumUpdating();
                        break;
                }
            }
        }else
        {
            switch(currentEnvironment)
            {
                case environment.G0:
                    G0Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G1:
                    G1Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G2:
                    G2Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G3:
                    G3Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G4:
                    G4Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G5:
                    G5Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G6:
                    G6Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G7:
                    G7Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G8:
                    G8Visualizer.StopSpectrumUpdating();
                    break;
                case environment.G9:
                    G9Visualizer.StopSpectrumUpdating();
                    break;
            }
            isSpectrumUpdating = false;
        }

    }

    public void compareSongLists()
    {
        // Disable all game objects in the current environment
        GameObject environmentTransform = transform.Find(currentEnvironment.ToString()).gameObject;
        if(environmentTransform)
        {
            for(int i = 0; i < environmentTransform.transform.childCount; i++)
            {
                GameObject child = environmentTransform.transform.GetChild(i).gameObject;
                child.SetActive(false);
            }
            environmentTransform.SetActive(false);
        }

        string currentSongName = audioManager.MusicList.SongPlaylist[AudioPlaybackManager.SongNumber].songName;
        SongData result = featureManager.AnalyzedSongList.Find(x => x.Name == currentSongName + ".wav");
        Debug.Log( $"Song is already analyzed. Song title: {currentSongName}, song feature group: {result.FeatureGroup}" );
        currentEnvironment = (environment)result.FeatureGroup;

        // Enable all GameObjects in the new current environment
        environmentTransform = transform.Find(currentEnvironment.ToString()).gameObject;
        if(environmentTransform)
        {
            for(int i = 0; i < environmentTransform.transform.childCount; i++)
            {
                GameObject child = environmentTransform.transform.GetChild(i).gameObject;
                child.SetActive(true);
            }
            environmentTransform.SetActive(true);
        }
    }
}
