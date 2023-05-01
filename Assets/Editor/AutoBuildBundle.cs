using UnityEngine;
using UnityEditor;
using System.IO;

public class AutoBuildBundle
{
    [MenuItem("File/Set Version")]
    public static void SetVersion()
    {
        VersionWindow window = EditorWindow.GetWindow<VersionWindow>(true, "Wait for setting version...");
        window.ShowUtility();
    }
    [MenuItem("File/Build Project")]
    public static void BuildWithBundle()
    {
        // build asset bundles first
        BuildBundle(GetBuildTarget());
        // Set Version
        VersionWindow window = EditorWindow.GetWindow<VersionWindow>(true, "Wait for setting version...");
        window.callback += () =>
        {
            // then build the project
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetEnabledScenes();
            buildPlayerOptions.locationPathName = GetBuildPath();
            buildPlayerOptions.target = GetBuildTarget();
            buildPlayerOptions.options = BuildOptions.None;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("Build Project...");
        };
        window.ShowUtility();
    }
    [MenuItem("File/Build Project with Debug")]
    public static void BuildWithBundleWithDebug()
    {
        // build asset bundles first
        BuildBundle(GetBuildTarget());
        // Set Version
        VersionWindow window = EditorWindow.GetWindow<VersionWindow>(true, "Wait for setting version...");
        window.callback += () =>
        {
            // then build the project
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.scenes = GetEnabledScenes();
            buildPlayerOptions.locationPathName = GetBuildPath();
            buildPlayerOptions.target = GetBuildTarget();
            buildPlayerOptions.options = BuildOptions.Development | BuildOptions.AllowDebugging;
            BuildPipeline.BuildPlayer(buildPlayerOptions);
            Debug.Log("Build Debugged Project...");
        };
        window.ShowUtility();
    }

    static string GetBuildPath()
    {
        string path = "Builds/";
        string extension = "";
        if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android)
        {
            path += "Android/";
            extension = ".apk";
        }
        else if (EditorUserBuildSettings.activeBuildTarget == BuildTarget.iOS)
        {
            path += "iOS/";
            extension = ".ipa";
        }
        else
        {
            path += "Windows/";
            extension = ".exe";
        }
        return path + Application.productName + extension;
    }

    static BuildTarget GetBuildTarget()
    {
        return EditorUserBuildSettings.activeBuildTarget;
    }

    static string[] GetEnabledScenes()
    {
        var scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }

    static void BuildBundle(BuildTarget buildTarget)
    {
        AssetBundleTools.BuildBundle(buildTarget);
    }
}
