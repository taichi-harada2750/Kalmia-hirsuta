"""
カメラ診断スクリプト。
インデックス0〜5を DSHOW / 既定(MSMF等) の両バックエンドで開き、
実際にフレームが読めるか・解像度・明るさ(平均輝度)を報告する。

使い方: python camera_probe.py
"""
import cv2
import time

BACKENDS = [
    (cv2.CAP_DSHOW, "DSHOW"),
    (cv2.CAP_MSMF, "MSMF"),
    (cv2.CAP_ANY, "ANY"),
]
MAX_INDEX = 5

print("=== カメラ診断開始 ===\n")

found_any = False
for index in range(MAX_INDEX + 1):
    for backend, name in BACKENDS:
        cap = cv2.VideoCapture(index, backend)
        opened = cap.isOpened()
        if not opened:
            cap.release()
            continue

        # フレーム読み取りを数回試す
        ok = False
        mean_brightness = 0.0
        w = h = 0
        for _ in range(10):
            ret, frame = cap.read()
            if ret and frame is not None:
                ok = True
                h, w = frame.shape[:2]
                mean_brightness = float(frame.mean())
                break
            time.sleep(0.05)

        if ok:
            found_any = True
            dark = "  ⚠️真っ黒(輝度ほぼ0)" if mean_brightness < 5 else ""
            print(f"✅ index={index:<2} backend={name:<6} "
                  f"解像度={w}x{h} 平均輝度={mean_brightness:.1f}{dark}")
        else:
            print(f"△ index={index:<2} backend={name:<6} 開いたがフレーム取得不可（占有中の可能性）")

        cap.release()

print("\n=== 診断終了 ===")
if not found_any:
    print("❌ どのインデックス/バックエンドでもフレームを取得できませんでした。")
    print("   → カメラが物理的に認識されていない、ドライバ、または他アプリが占有中の可能性。")
else:
    print("上で ✅ が付いた index / backend の組み合わせを使ってください。")
    print("平均輝度がほぼ0(真っ黒)の場合は露出固着 → Windowsカメラアプリで一度開くとリセットされます。")
