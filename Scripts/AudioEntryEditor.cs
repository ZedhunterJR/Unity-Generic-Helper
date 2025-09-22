#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(NPCSoundEntry))]
public class NPCSoundEntryEditor : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.BeginProperty(position, label, property);

        float third = position.width / 3f;
        var soundTypeProp = property.FindPropertyRelative("audioName");
        var clipProp = property.FindPropertyRelative("clip");
        var lengthProp = property.FindPropertyRelative("clipLength");

        var soundTypeRect = new Rect(position.x, position.y, third - 4f, position.height);
        var clipRect = new Rect(position.x + third + 2f, position.y, third - 4f, position.height);
        var lengthRect = new Rect(position.x + 2 * third + 4f, position.y, third - 4f, position.height);

        // Draw fields
        EditorGUI.PropertyField(soundTypeRect, soundTypeProp, GUIContent.none);

        EditorGUI.BeginChangeCheck();
        EditorGUI.PropertyField(clipRect, clipProp, GUIContent.none);
        if (EditorGUI.EndChangeCheck())
        {
            // Update length automatically when clip is assigned/changed
            var clip = clipProp.objectReferenceValue as AudioClip;
            lengthProp.floatValue = clip != null ? clip.length : 0f;
        }

        // Draw clip length (read-only)
        EditorGUI.LabelField(lengthRect, lengthProp.floatValue.ToString("F2") + "s");

        EditorGUI.EndProperty();
    }
}

[CustomEditor(typeof(AudioEntry), true)]
public class AudioEntryEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        if (GUILayout.Button("Import Audio Clips from Folder"))
        {
            ImportAudioClips((AudioEntry)target);
        }
    }

    private void ImportAudioClips(AudioEntry audioEntry)
    {
        string AudioFolderPath = audioEntry.audioPath;
        if (!Directory.Exists(AudioFolderPath))
        {
            Debug.LogWarning($"Directory not found: {AudioFolderPath}");
            return;
        }

        string[] validExtensions = { ".wav", ".mp3", ".ogg", ".aiff" };
        string[] files = Directory.GetFiles(AudioFolderPath, "*.*", SearchOption.AllDirectories);
        Array.Sort(files, (a, b) =>
        {
            string nameA = Path.GetFileNameWithoutExtension(a);
            string nameB = Path.GetFileNameWithoutExtension(b);

            // Try to parse the number at the start of the filename
            int.TryParse(GetLeadingNumber(nameA), out int numA);
            int.TryParse(GetLeadingNumber(nameB), out int numB);

            return numA.CompareTo(numB);
        });
        audioEntry.ClearEntries();

        int count = 0;
        foreach (string path in files)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (System.Array.Exists(validExtensions, e => e == ext))
            {
                string assetPath = path.Replace("\\", "/");
                AudioClip clip = AssetDatabase.LoadAssetAtPath<AudioClip>(assetPath);

                if (clip != null)
                {
                    audioEntry.AddEntry(clip);
                    count++;
                }
            }
        }

        Debug.Log($"Imported {count} audio clips from '{AudioFolderPath}'.");
        EditorUtility.SetDirty(audioEntry);
    }

    private static string GetLeadingNumber(string input)
    {
        string digits = "";
        foreach (char c in input)
        {
            if (char.IsDigit(c)) digits += c;
            else break;
        }
        return digits;
    }
}
#endif
