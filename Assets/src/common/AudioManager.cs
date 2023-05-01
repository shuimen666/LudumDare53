using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager INSTANCE;
    public static AudioManager get() { return INSTANCE; }

    private AudioSource audioSource;
    private Dictionary<string, AudioClip> clipCache;

    void Awake()
    {
        INSTANCE = this;
        audioSource = GetComponent<AudioSource>();
        clipCache = new Dictionary<string, AudioClip>();
        Init();
    }

    private void Init()
    {
        LoadAudio("put_chess");
        LoadAudio("fire");
    }

    public AudioClip LoadAudio(string audioName)
    {
        if (!clipCache.ContainsKey(audioName)) {
            clipCache.Add(audioName, AssetLoader.get().LoadResource<AudioClip>("audios", audioName));
        }
        return clipCache[audioName];
    }
    public void PlayAudio(string audioName)
    {
        audioSource.PlayOneShot(LoadAudio(audioName));
    }
    public void PlayAudio(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }
    public void PlayBackgroundAudio(string audioName)
    {
        audioSource.clip = LoadAudio(audioName);
        audioSource.Play();
    }


    public void PlayAudio(sm.audio audioType)
    {
        switch (audioType)
        {
            /*case sm.audio.BGM:
                PlayBackgroundAudio("bgm");
                break;
            case sm.audio.PUT_CHESS:
                PlayAudio("put_chess");
                break;*/
        }
    }
}
