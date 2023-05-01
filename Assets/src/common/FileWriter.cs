using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class FileWriter : MonoBehaviour
{
    private static FileWriter INSTANCE = null;
    public static FileWriter get() {
        if (INSTANCE == null) INSTANCE = new FileWriter();
        return INSTANCE;
    }

    public void WriteInfoIn(List<string> info, string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            writer.WriteLine(string.Join(",", info));
        }
    }
    public void WriteInfoIn(List<List<string>> info, string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            foreach (List<string> list in info)
            {
                writer.WriteLine(string.Join(",", list));
            }
        }
    }
    public void WriteJsonfile(string jsonData, string fileName, bool overwrite=false)
    {
        if (!fileName.EndsWith(".json")) fileName = fileName + ".json";
        string filePath = Application.dataPath + "/" + fileName;
        using (StreamWriter writer = new StreamWriter(filePath, !overwrite))
        {
            writer.Write(jsonData);
        }
    }
    public string ReadJsonFile(string fileName)
    {
        if (!fileName.EndsWith(".json")) fileName = fileName + ".json";
        string filePath = Application.dataPath + "/" + fileName;
        using(StreamReader reader = new StreamReader(filePath, true))
        {
            return reader.ReadToEnd();
        }
    }
    public void CheckDirectory(string folderPath)
    {
        folderPath = Application.dataPath + "/" + folderPath;
        if(!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }
    }
    public async void LoadAllJsonFilesAsync(string folderPath, Action<Dictionary<string, string>> callback, CancellationToken cancelToken = default)
    {
        Dictionary<string, string> jsonDictionary = new Dictionary<string, string>();
        await LoadAllJsonFilesAsync_(folderPath, jsonDictionary, cancelToken);
        callback?.Invoke(jsonDictionary);
    }
    private async Task LoadAllJsonFilesAsync_(string folderPath, Dictionary<string, string> jsonDictionary, CancellationToken cancelToken = default)
    {
        folderPath = Application.dataPath + "/" + folderPath;
        string[] jsonFilePaths = Directory.GetFiles(folderPath, "*.json");
        List<Task> tasks = new List<Task>();
        foreach(string filePath in jsonFilePaths)
        {
            tasks.Add(Task.Run(async () =>
            {
                string jsonString = await File.ReadAllTextAsync(filePath, cancelToken);
                string fileName = Path.GetFileNameWithoutExtension(filePath);
                if (cancelToken.IsCancellationRequested) return;
                if(!jsonDictionary.ContainsKey(fileName))
                {
                    jsonDictionary.Add(fileName, jsonString);
                }
            }, cancelToken));
        }
        await Task.WhenAll(tasks);
    }
    public bool DeleteJsonFile(string fileName)
    {
        if (!fileName.EndsWith(".json")) fileName += ".json";
        return DeleteFile(fileName);
    }
    public bool DeleteFile(string fileName)
    {
        string filePath = Application.dataPath + "/" + fileName;
        if (!File.Exists(filePath)) return false;
        File.Delete(filePath);
        return true;
    }
}