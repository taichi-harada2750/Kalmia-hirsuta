using UnityEngine;

public class CommonCloseDialog : MonoBehaviour
{
    [Header("確認ダイアログのUIパネル")]
    [SerializeField] private GameObject dialogPanel;

    [Header("リセット処理スクリプト")]
    [SerializeField] private SceneResetRequester resetRequester;

    public void ShowDialog()
    {
        dialogPanel.SetActive(true);
    }

    public void OnConfirm()
    {
        dialogPanel.SetActive(false);
        resetRequester.RequestReset(); // LoadingSceneへ移動
    }

    public void OnCancel()
    {
        dialogPanel.SetActive(false);
    }
}
