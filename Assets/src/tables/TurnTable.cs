using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnTable : BaseTable
{
    protected static TurnTable INSTANCE = null;
    public static TurnTable get()
    {
        if (INSTANCE == null) { INSTANCE = new TurnTable(); }
        return INSTANCE;
    }

    private int maxTurnID;

    public override void InitTable()
    {
        ReadCsv("turn.csv");
        SetMaxTurn();

    }
    private void SetMaxTurn()
    {
        maxTurnID = 1;
        foreach (var id in ids)
        {
            maxTurnID = Mathf.Max(maxTurnID, id);
        }
    }
    public Dictionary<string, string> getTurn(int id)
    {
        return getLine(id);
    }
    public int getMaxTurnID() { return maxTurnID; }
}
