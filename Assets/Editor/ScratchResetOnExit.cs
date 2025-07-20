using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;

[InitializeOnLoad]
public static class ScratchResetOnExit
{
    static ScratchResetOnExit()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode)
        {
            // ScratchPainterのClearMaskを探して呼び出す
            var scratchPainters = GameObject.FindObjectsOfType<ScratchPainter>();
            foreach (var painter in scratchPainters)
            {
                Debug.Log("[ScratchResetOnExit] Resetting mask after play mode.");
                painter.ClearMask();
            }
        }
    }
}
