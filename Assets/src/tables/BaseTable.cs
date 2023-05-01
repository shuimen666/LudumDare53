using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class BaseTable
{
    protected List<List<string>> dataArray;
    protected Dictionary<int, Dictionary<string, string>> dataDict;
    protected List<int> ids;

    protected void ReadCsv(string fileName)
    {
#if UNITY_EDITOR
        fileName = "Assets/BundleResources/Tables/" + fileName;
        using (StreamReader reader = new StreamReader(fileName))
#else
        TextAsset textAsset = AssetLoader.get().LoadResource<TextAsset>("tables", fileName);
        using (StreamReader reader = new StreamReader(new MemoryStream(textAsset.bytes)))
#endif
        {
            dataArray = new List<List<string>>();
            dataDict = new Dictionary<int, Dictionary<string, string>>();
            ids = new List<int>();

            string[] headers = reader.ReadLine().Split(',');

            int row = 0;
            while (!reader.EndOfStream)
            {
                List<string> line = reader.ReadLine().Split(',').ToList();

                dataArray.Add(line);

                Dictionary<string, string> dict = new Dictionary<string, string>();
                int id = -1;
                for (int i = 0; i < headers.Length; i++)
                {
                    if(!string.IsNullOrEmpty(headers[i]))
                    {
                        dict[headers[i]] = line[i];
                    }
                    if (headers[i] == "id")
                    {
                        id = sm.ParseToInt(line[i]);
                    }
                }
                ids.Add(id);
                dataDict[id] = dict;


                row++;
            }
        }
    }
    protected BaseTable()
    {
        InitTable();
    }
    public virtual void InitTable() { }
    protected Dictionary<string, string> getLine(int id) { return dataDict[id]; }
    protected List<List<string>> getDataArray() { return dataArray; }
    protected string getValue(int id, string key) { return dataDict[id][key]; }
}
