using UnityEngine;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("SE用AudioSource（2ch制）")]
    public AudioSource primarySE;
    public AudioSource backupSE;

    [Header("BGM用AudioSource（ループ用）")]
    public AudioSource bgmSource;

    [Header("固定SE")]
    public AudioClip clickSE;

    [System.Serializable]
    public class SoundEntry
    {
        public string key;
        public AudioClip clip;
    }

    [Header("追加SE（任意キー）")]
    public List<SoundEntry> sounds = new List<SoundEntry>();

    [Header("BGMリスト（任意キー）")]
    public List<SoundEntry> bgms = new List<SoundEntry>();

    private Dictionary<string, AudioClip> seDict;
    private Dictionary<string, AudioClip> bgmDict;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            BuildDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void BuildDictionaries()
    {
        seDict = new Dictionary<string, AudioClip>();
        foreach (var entry in sounds)
        {
            if (!seDict.ContainsKey(entry.key))
                seDict.Add(entry.key, entry.clip);
        }

        bgmDict = new Dictionary<string, AudioClip>();
        foreach (var entry in bgms)
        {
            if (!bgmDict.ContainsKey(entry.key))
                bgmDict.Add(entry.key, entry.clip);
        }
    }

    // --- SE ---
    public void PlayClick()
    {
        if (clickSE != null) PlaySE(clickSE);
    }

    public void PlaySE(string key)
    {
        if (key == "click")
        {
            PlayClick();
            return;
        }

        if (seDict.TryGetValue(key, out var clip) && clip != null)
        {
            PlaySE(clip);
        }
        else
        {
            Debug.LogWarning($"[SoundManager] SE '{key}' が見つかりません");
        }
    }

    public void PlaySE(AudioClip clip)
    {
        if (clip == null) return;

        if (primarySE != null && !primarySE.isPlaying)
        {
            primarySE.PlayOneShot(clip);
        }
        else if (backupSE != null)
        {
            backupSE.PlayOneShot(clip);
        }
        else
        {
            primarySE?.PlayOneShot(clip);
        }
    }

    // --- BGM ---
    public void PlayBGM(string key, bool loop = true)
    {
        if (bgmDict.TryGetValue(key, out var clip) && clip != null)
        {
            bgmSource.clip = clip;
            bgmSource.loop = loop;
            bgmSource.Play();
        }
        else
        {
            Debug.LogWarning($"[SoundManager] BGM '{key}' が見つかりません");
        }
    }

    public void StopBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Stop();
        }
    }

    public void PauseBGM()
    {
        if (bgmSource.isPlaying)
        {
            bgmSource.Pause();
        }
    }

    public void ResumeBGM()
    {
        if (!bgmSource.isPlaying)
        {
            bgmSource.Play();
        }
    }
}
