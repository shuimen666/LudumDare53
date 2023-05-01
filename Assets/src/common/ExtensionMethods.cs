using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public static class ExtensionMethods
{
    public static void Shuffle<T>(this List<T> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while(n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    public static T Dequque<T>(this List<T> list)
    {
        int n = list.Count;
        if (n == 0) return default;
        T elem = list[n - 1];
        list.RemoveAt(n - 1);
        return elem;
    }
    public static List<string> ToList(this string str)
    {
        List<string> tmp = new List<string>();
        tmp.Add(str);
        return tmp;
    }
    public static void SetActive(this Transform transform, bool active)
    {
        transform.gameObject.SetActive(active);
    }
    public static void SetActive(this UIWidget widget, bool active) 
    {
        widget.gameObject.SetActive(active);
    }
    public static void SetActive(this UIWidgetContainer widget, bool active)
    {
        widget.gameObject.SetActive(active);
    }
    public static Transform Find(this GameObject gameObject, string n)
    {
        return gameObject.transform.Find(n);
    }
    public static T GetComponent<T>(this Transform transform, string path) where T: Component
    {
        Transform t = transform.Find(path);
        if (t != null)
        {
            return t.GetComponent<T>();
        }
        return null;
    }
    public static T GetComponent<T>(this GameObject gameObject, string path) where T : Component
    {
        return gameObject.transform.GetComponent<T>(path);
    }
}
