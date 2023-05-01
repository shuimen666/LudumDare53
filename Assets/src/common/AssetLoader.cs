using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssetLoader : MonoBehaviour
{
    private static AssetLoader INSTANCE;
    public static AssetLoader get() { return INSTANCE; }
    private Dictionary<string, AssetBundle> abCache;
    private AssetBundle mainAB = null;
    private AssetBundleManifest mainManifest = null;
    private string basePath
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return Application.dataPath + "/StreamingAssets/";
#elif UNITY_IPHONE
            return Application.dataPath + "/Raw/";
#elif UNITY_ANDROID
            return Application.streamingAssetsPath + "/";
#endif
        }
    }
    private string mainABName
    {
        get
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            return "StreamingAssets";
#elif UNITY_IPHONE
            return "IOS";
#elif UNITY_ANDROID
            return "StreamingAssets";
#endif
        }
    }
    private void Awake()
    {
        INSTANCE = this;
        Init();
    }
    private void Init()
    {
        abCache = new Dictionary<string, AssetBundle>();
    }
    private AssetBundle LoadABPackage(string abName)
    {
        abName = abName.ToLower();
        AssetBundle ab;
        if (mainAB == null)
        {
            mainAB = AssetBundle.LoadFromFile(basePath + mainABName);
            mainManifest = mainAB.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        }
        string[] dependencies = mainManifest.GetAllDependencies(abName);
        for (int i = 0; i < dependencies.Length; i++)
        {
            if (!abCache.ContainsKey(dependencies[i]))
            {
                ab = AssetBundle.LoadFromFile(basePath + dependencies[i]);
                abCache.Add(dependencies[i], ab);
                Debug.LogError("Load bundle "+dependencies[i]);
            }
        }
        if (abCache.ContainsKey(abName)) return abCache[abName];
        else
        {
            ab = AssetBundle.LoadFromFile(basePath + abName);
            abCache.Add(abName, ab);
            Debug.LogError("Load bundle " + abName);
            return ab;
        }
    }
    public T LoadResource<T>(string abName, string resName) where T : Object
    {
        Debug.Log(abName);
        AssetBundle ab = LoadABPackage(abName);
        return ab.LoadAsset<T>(resName);
    }
    public Object LoadResource(string abName, string resName)
    {
        AssetBundle ab = LoadABPackage(abName);
        return ab.LoadAsset(resName);
    }
    public Object LoadResource(string abName, string resName, System.Type type)
    {
        AssetBundle ab = LoadABPackage(abName);
        return ab.LoadAsset(resName, type);
    }
    public void LoadResourceAsync(string abName, string resName, System.Action<Object> finishLoadObjectHandler)
    {
        AssetBundle ab = LoadABPackage(abName);
        StartCoroutine(LoadRes(ab, resName, finishLoadObjectHandler));
    }
    private IEnumerator LoadRes(AssetBundle ab, string resName, System.Action<Object> finishLoadObjectHandler)
    {
        if (ab == null) yield break;
        AssetBundleRequest abr = ab.LoadAssetAsync(resName);
        yield return abr;
        finishLoadObjectHandler(abr.asset);
    }
    public void LoadResourceAsync(string abName, string resName, System.Type type, System.Action<Object> finishLoadObjectHandler)
    {
        AssetBundle ab = LoadABPackage(abName);
        StartCoroutine(LoadRes(ab, resName, type, finishLoadObjectHandler));
    }
    private IEnumerator LoadRes(AssetBundle ab, string resName, System.Type type, System.Action<Object> finishLoadObjectHandler)
    {
        if (ab == null) yield break;
        AssetBundleRequest abr = ab.LoadAssetAsync(resName, type);
        yield return abr;
        finishLoadObjectHandler(abr.asset);
    }
    public void LoadResourceAsync<T>(string abName, string resName, System.Action<Object> finishLoadObjectHandler) where T : Object
    {
        AssetBundle ab = LoadABPackage(abName);
        StartCoroutine(LoadRes<T>(ab, resName, finishLoadObjectHandler));
    }

    private IEnumerator LoadRes<T>(AssetBundle ab, string resName, System.Action<Object> finishLoadObjectHandler) where T : Object
    {
        if (ab == null) yield break;
        AssetBundleRequest abr = ab.LoadAssetAsync<T>(resName);
        yield return abr;
        finishLoadObjectHandler(abr.asset as T);
    }

    public void UnLoad(string abName)
    {
        if (abCache.ContainsKey(abName))
        {
            abCache[abName].Unload(false);
            abCache.Remove(abName);
        }
    }
    public void UnLoadAll()
    {
        AssetBundle.UnloadAllAssetBundles(false);
        abCache.Clear();
        mainAB = null;
        mainManifest = null;
    }
}
