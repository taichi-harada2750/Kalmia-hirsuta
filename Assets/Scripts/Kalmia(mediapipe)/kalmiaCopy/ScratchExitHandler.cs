using UnityEngine;

public class ScratchExitHandler : MonoBehaviour
{
    public ScratchPainter painter;

    public void ResetMask()
    {
        if (painter != null)
        {
            Debug.Log("[ScratchExitHandler] ResetMask called.");
            painter.ClearMask();
        }
        else
        {
            Debug.LogWarning("[ScratchExitHandler] painter が設定されていません。");
        }
    }
}
