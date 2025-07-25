using UnityEngine;

public class BGMChanger : MonoBehaviour
{
    [Header("切り替えるBGMキー（SoundManagerのbgmsに登録されたキー）")]
    public string bgmKey = "mainTheme";
    public bool playImmediately = true;

    // IconTriggerのonClickから呼ばれる
    public void ChangeBGM()
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBGM(bgmKey, true);
            Debug.Log($"BGMを {bgmKey} に変更しました");
        }
    }
}
