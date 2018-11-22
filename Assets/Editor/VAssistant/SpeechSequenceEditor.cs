using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SpeechSequence))]
public class SpeechSequenceEditor : Editor
{

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        SpeechSequence speechSequence = (SpeechSequence)target;
        
        

        //EditorGUI.PropertyField(new Rect(0,300,500,30),)
    }
}
