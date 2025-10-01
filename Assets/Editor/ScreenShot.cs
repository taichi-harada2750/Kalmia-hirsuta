#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

public static class ScreenshotUtil
{
    private const string outputDirectory = "Screenshots/";
    private const string fileNamePrefix = "screenshot";

    // ctrl(macならcommand)+alt+Sをショートカットに設定
    [MenuItem("Tools/Screenshot %&S", false)]
    public static void CaptureScreenshot()
    {
        // 出力先ディレクトリがなければ作成する
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }

        var now = DateTime.Now;
        // 出力ファイル名を指定
        var fileName = $"{fileNamePrefix}_{now.Year}_{now.Month}_{now.Day}_{now.Hour}_{now.Minute}_{now.Second}.png";
        // 出力パスを指定
        var outputPath = Path.Combine(outputDirectory, fileName);
        ScreenCapture.CaptureScreenshot(outputPath);
        // 撮影したことをログで通知
        Debug.Log($"Captured a screenshot: {fileName}");
    }
}
#endif
