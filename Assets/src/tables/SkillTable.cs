using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTable : BaseTable
{
    protected static SkillTable INSTANCE = null;
    public static SkillTable get()
    {
        if (INSTANCE == null) { INSTANCE = new SkillTable(); }
        return INSTANCE;
    }

    private List<int> ablePool;

    public override void InitTable()
    {
        ReadCsv("skill.csv");
        SetPool();

    }
    private void SetPool()
    {
        ablePool = new List<int>();
        foreach (var id in ids)
        {
            /*string inPool = getValue(id, "in_pool");
            if (inPool == "" || sm.ParseToInt(inPool) == 0) continue;*/
            ablePool.Add(id);
        }
    }
    public Dictionary<string, string> getSkill(int id)
    {
        return getLine(id);
    }
    public List<int> getPool() { return ablePool; }
    public string getSprite(int id)
    {
        return getLine(id)["sprite"];
    }
    public string getDesc(int id)
    {
        return getLine(id)["desc"];
    }
}
