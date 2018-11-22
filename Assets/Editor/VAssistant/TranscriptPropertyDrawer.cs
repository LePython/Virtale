using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(Transcript))]
public class TranscriptPropertyDrawer: PropertyDrawer
{
    // SerializedProperty audioPath, text, delay;
    // string name;
    // bool cache = false;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label) {
        EditorGUI.BeginProperty(position, label, property);

        EditorGUI.Vector3Field(position, label, property.vector3Value);

        EditorGUI.EndProperty();

        // if(!cache)
        // {
        //     name = property.displayName;

        //     property.Next(true);
        //     audioPath = property.Copy();
        //     property.Next(true);
        //     text = property.Copy();
        //     property.Next(true);
        //     delay = property.Copy();

        //     cache = true;
        // }

        // Rect contentPosition = EditorGUI.PrefixLabel(position, new GUIContent(name));

        // //Check if there is enough space to put the name on the same line (to save space)
        // if (position.height > 16f)
        // {
        //     position.height = 16f;
        //     EditorGUI.indentLevel += 1;
        //     contentPosition = EditorGUI.IndentedRect(position);
        //     contentPosition.y += 18f;
        // }

        // float half = contentPosition.width / 2;
        // GUI.skin.label.padding = new RectOffset(3, 3, 6, 6);

        // EditorGUIUtility.labelWidth = 16f;
        // contentPosition.width *= 0.5f;
        // EditorGUI.indentLevel = 0;

        // EditorGUI.BeginProperty(contentPosition, label, audioPath);
        // {
        //     EditorGUI.BeginChangeCheck();
        //     string path = EditorGUI.TextField(contentPosition, new GUIContent("AudioPath"), audioPath.stringValue);
        //     if(EditorGUI.EndChangeCheck())
        //     {
        //         audioPath.stringValue = path;
        //     }
        // }
    }
}
