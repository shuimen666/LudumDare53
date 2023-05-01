using UnityEditor;
using UnityEngine;
using System.IO;
using System.Diagnostics;

public class GenerateAtlasMenuItem
{
    [MenuItem("Assets/Generate Selected Atlas", false, 100000)]
    static void GenerateAtlas()
    {
        string selectedFolderPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (string.IsNullOrEmpty(selectedFolderPath))
            selectedFolderPath = Selection.activeObject.ToString();
        string atlasName = Path.GetFileName(selectedFolderPath);
        string imagesFolderPath = Path.Combine(selectedFolderPath, "images");

        if (Directory.Exists(imagesFolderPath))
        {
            string[] imageFolders = Directory.GetDirectories(imagesFolderPath);
            string inputFilePath = "";

            foreach (string folder in imageFolders)
            {
                string[] files = Directory.GetFiles(folder, "*.png");

                foreach (string file in files)
                {
                    inputFilePath += file + " ";
                }
            }

            string sheetPath = Path.Combine(selectedFolderPath, atlasName + ".png");
            string dataPath = Path.Combine(selectedFolderPath, atlasName + ".txt");
            string command = "--quiet --format unity --disable-rotation --sheet " + sheetPath + " --data " + dataPath + " " + imagesFolderPath;

            UnityEngine.Debug.LogError(command);
            ExecuteTexturePacker(command);
        }
        else
        {
            UnityEngine.Debug.LogError("Selected folder does not contain an 'images' subfolder.");
        }
    }

    static void ExecuteTexturePacker(string arguments)
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = "D:/TexturePacker3.0.9/bin/TexturePacker.exe";
        startInfo.Arguments = arguments;
        startInfo.UseShellExecute = false;
        startInfo.RedirectStandardOutput = true;
        startInfo.CreateNoWindow = true;

        using (Process process = Process.Start(startInfo))
        {
            process.WaitForExit();
            string output = process.StandardOutput.ReadToEnd();
            UnityEngine.Debug.Log(output);
        }
    }
}
