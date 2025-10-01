using UnityEngine;
using System.IO;

public class ScreenshotCapture : MonoBehaviour
{
    [Tooltip("保存フォルダ名（プロジェクト直下に生成されます）")]
    public string folderName = "Screenshots";

    [Tooltip("ショートカットキー")]
    public KeyCode captureKey = KeyCode.F12;

    void Update()
    {
        if (Input.GetKeyDown(captureKey))
        {
            TakeScreenshot();
        }
    }

    void TakeScreenshot()
    {
        string folderPath = Path.Combine(Application.dataPath, "..", folderName);

        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string timestamp = System.DateTime.Now.ToString("yyyyMMdd_HHmmss");
        string filePath = Path.Combine(folderPath, $"screenshot_{timestamp}.png");

        ScreenCapture.CaptureScreenshot(filePath);
        Debug.Log($"📸 スクリーンショット保存: {filePath}");
    }
}
