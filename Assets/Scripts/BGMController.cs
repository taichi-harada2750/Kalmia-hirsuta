using UnityEngine;

public class BGMController : MonoBehaviour
{
    [Header("現在のBGMキー（SoundManagerのbgmsに登録されたキー）")]
    public string currentBGMKey = "mainTheme";
    public bool playOnStart = true;
    public bool loop = true;

    private bool isMuted = false;

    void Start()
    {
        if (playOnStart)
        {
            PlayBGM();
        }
    }

    void Update()
    {
        // 展示用: Mキーでミュート切り替え
        if (Input.GetKeyDown(KeyCode.M))
        {
            ToggleMute();
        }
    }

    // --- 再生/停止 ---
    public void PlayBGM()
    {
        if (SoundManager.Instance != null && !string.IsNullOrEmpty(currentBGMKey))
        {
            SoundManager.Instance.PlayBGM(currentBGMKey, loop);
        }
    }

    public void StopBGM()
    {
        SoundManager.Instance?.StopBGM();
    }

    // --- 設定アプリからBGM変更用 ---
    public void ChangeBGM(string newKey)
    {
        currentBGMKey = newKey;

        // 再生中なら切り替え
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(currentBGMKey, loop);
        }
    }

    // --- 展示用ミュート ---
    public void ToggleMute()
    {
        isMuted = !isMuted;

        if (SoundManager.Instance != null)
        {
            if (isMuted)
            {
                SoundManager.Instance.bgmSource.volume = 0f;
                SoundManager.Instance.primarySE.volume = 0f;
                SoundManager.Instance.backupSE.volume = 0f;
            }
            else
            {
                SoundManager.Instance.bgmSource.volume = 1f;
                SoundManager.Instance.primarySE.volume = 1f;
                SoundManager.Instance.backupSE.volume = 1f;
            }
        }

        Debug.Log(isMuted ? "🔇 ミュート ON" : "🔊 ミュート OFF");
    }
}
