using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslationTable : BaseTable
{
    protected static TranslationTable INSTANCE = null;
    public static TranslationTable get()
    {
        if (INSTANCE == null) { INSTANCE = new TranslationTable(); }
        return INSTANCE;
    }
    private Dictionary<string, string> chsDict, engDict;
    public override void InitTable()
    {
        ReadCsv("translation.csv");
        SetPureDict();
    }
    private void SetPureDict()
    {
        chsDict = new Dictionary<string, string>();
        engDict = new Dictionary<string, string>();
        List<List<string>> arr = getDataArray();
        foreach(var line in arr)
        {
            if (line[1] != "")
            {
                chsDict.Add(line[1], line[2]);
                engDict.Add(line[1], line[3]);
            }
        }
    }
    public string getTranslation(string id)
    {
        var dict = sm.isEng ? engDict : chsDict;
        if (dict.TryGetValue(id, out string desc)) return desc;
        else return "";
    }
}
