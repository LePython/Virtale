using UnityEngine;
using UnityEditor;
using UnityEngine.AudioManager;

[CustomEditor(typeof(AudioPlaybackManager))]
public class AudioPlaybackManagerEditor : Editor {


    public override void OnInspectorGUI() {
        base.OnInspectorGUI();

        AudioPlaybackManager audioManager = (AudioPlaybackManager)target;        

        GUILayout.BeginHorizontal();

        if(GUILayout.Button("Last Song"))
        {
            audioManager.LastSong();
        }
        if(GUILayout.Button("Pause/Play"))
        {
            audioManager.PauseAudio();
        }
        if(GUILayout.Button("Next Song"))
        {
            audioManager.NextSong();
        }

        GUILayout.EndHorizontal();
    }
}