using UnityEditor;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

public class AssetBundleTools
{
    static void SetName()
    {
        string basePath = "Assets/BundleResources/";
        DirectoryInfo directory = new DirectoryInfo(basePath);
        DirectoryInfo[] directories = directory.GetDirectories();
        foreach (DirectoryInfo dir in directories)
        {
            string folderName = dir.Name.ToLower();
            FileInfo[] files = dir.GetFiles("*.*", SearchOption.AllDirectories);
            foreach (FileInfo file in files)
            {
                if (Path.GetExtension(file.Name) != ".meta")
                {
                    string assetPath = file.FullName.Replace(@"\", "/").Replace(Application.dataPath, "Assets");
                    AssetImporter importer = AssetImporter.GetAtPath(assetPath);
                    importer.assetBundleName = folderName;
                }
            }
        }
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    public static void BuildBundle(BuildTarget buildTarget = BuildTarget.StandaloneWindows64)
    {
        SetName();
        string assetBundleDirectory = Application.streamingAssetsPath;
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, buildTarget);
        Debug.Log("Build Bundles...");
    }

    [MenuItem("Assets/Build AssetBundle(Android)")]
    public static void BuildBundleAndroid()
    {
        BuildBundle(BuildTarget.Android);
    }
    [MenuItem("Assets/Build AssetBundle")]
    public static void BuildBundleWindows()
    {
        BuildBundle(BuildTarget.StandaloneWindows64);
    }
}
