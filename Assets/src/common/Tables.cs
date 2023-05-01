using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class sm
{
    public class Tables
    {
        public SkillTable skill = SkillTable.get();
        public TurnTable turn = TurnTable.get();
        public TranslationTable translation = TranslationTable.get();
    }
    public static Tables tables = new Tables();

    public static int ParseToInt(string str)
    {
        if(int.TryParse(str, out int result))
        {
            return result;
        }
        else
        {
            Debug.LogError("Error Parse: " + str + " into int");
            return 0;
        }
    }
    public static string __(string key)
    {
        return tables.translation.getTranslation(key);
    }
    public static string __(int key)
    {
        return tables.translation.getTranslation(key.ToString());
    }
}
