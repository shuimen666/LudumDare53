using UnityEngine;
using UnityEditor;

public class VersionWindow : EditorWindow
{
    string version;
    public delegate void Callback();
    public Callback callback;

    public VersionWindow()
    {
        position = new Rect(100, 100, 200, 40);
    }

    private void OnEnable()
    {
        version = PlayerSettings.bundleVersion;
    }

    void OnGUI()
    {
        GUILayout.Label("Enter version number:");
        version = EditorGUILayout.TextField(version);

        if (GUILayout.Button("OK"))
        {
            PlayerSettings.bundleVersion = version;
            PlayerPrefs.SetString("version", version);
            Close();
            callback?.Invoke();
        }
    }
}
