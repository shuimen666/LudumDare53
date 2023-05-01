using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventControl
{
    private static EventControl INSTANCE = null;
    public static EventControl get() {
        if (INSTANCE == null) INSTANCE = new EventControl();
        return INSTANCE;
    }

    public Dictionary<sm.even, EventHandler> proxy = new Dictionary<sm.even, EventHandler>();

    public void AddEventListener(sm.even mes, EventHandler call)
    {
        if (!proxy.ContainsKey(mes)) proxy.Add(mes, call);
        else proxy[mes] += call;
    }
    public void RemoveEventListener(sm.even mes, EventHandler call)
    {
        if (!proxy.ContainsKey(mes)) return;
        proxy[mes] -= call;
    }
}

public partial class sm
{
    public static EventControl EventDispatcher = EventControl.get();
    public static void EventInvoke(sm.even even, object sender, EventArgs e)
    {
        if (!sm.EventDispatcher.proxy.ContainsKey(even)) return;
        sm.EventDispatcher.proxy[even]?.Invoke(sender, e);
    }
}