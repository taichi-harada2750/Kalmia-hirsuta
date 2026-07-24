# Kalmia-hirsuta

**Kalmia-hirsuta** は、Kinect以降（ポストKinect）の時代に向けた新しいNUI（Natural User Interface）として開発された卒業研究プロジェクトです。

本プロジェクトは、特別な深度センサーを必要とせず、一般的なWebカメラとAI技術を用いて両手の動きをトラッキングし、Unity上のアプリケーションと直感的にインタラクションできるシステムを構築しています。

## 概要・特徴

*   **ポストKinect NUI:** Kinectなどの専用デバイスに依存せず、MediaPipeを用いたカメラベースのハンドトラッキングによってNUIを実現しています。
*   **MediaPipeによる高精度トラッキング:** Pythonで動作するMediaPipeスクリプトが、ユーザーの両手（左右）の位置と「握る（Grab）」ジェスチャーを高精度に認識します。
*   **OSC通信によるUnity連携:** Python側で取得したトラッキングデータは、OSC（Open Sound Control）プロトコルを用いてローカルネットワーク（UDP 9000番ポート）経由でUnityへリアルタイムに送信されます。
*   **直感的なUI操作:** Unity側では受信した手の座標とジェスチャーをもとに、空間上のカーソル操作、リングUI（円形メニュー）のスクロール・選択、アイコンのホバー＆クリック操作などをシームレスに実行できます。

## システム構成

プロジェクトは大きく分けて以下の2つのコンポーネントで構成されています。

1.  **ハンドトラッキングシステム (Python)**
    *   スクリプト: `Assets/Scripts/mediapipe_osc_hand_tracker.py`
    *   Webカメラ映像からMediaPipe Handsを用いて指の関節位置を検出し、「握る」動作（指を曲げた状態）を判定します。
    *   左右それぞれの手のひらの座標（X, Y）と状態（Grab）をOSCメッセージとして送信します。
2.  **フロントエンド・アプリケーション (Unity)**
    *   `MediaPipeOSCReceiver` などのスクリプトでOSCデータを受信。
    *   `PalmDataManager` で手の位置情報を管理し、UIへのインタラクション（`IconTrigger`, `RingUIController` 等）へと変換します。

## 必要要件 (Requirements)

*   **Unity** (C# スクリプトおよび extOSC プラグインに依存)
*   **Python 3.x**
    *   `opencv-python` (cv2)
    *   `mediapipe`
    *   `python-osc`

## 実行方法 (Usage)

1.  Python環境に必要なライブラリをインストールします。
    ```bash
    pip install opencv-python mediapipe python-osc
    ```
2.  ハンドトラッキング用のPythonスクリプトを実行し、カメラを起動します。
    ```bash
    python Assets/Scripts/mediapipe_osc_hand_tracker.py
    ```
    ※カメラが起動し、トラッキング映像が表示されます。（終了する場合はウィンドウ上で `q` キーを押します）
3.  Unityで本プロジェクトを開き、シーンを再生（Play）します。
    *   カメラの前で手を動かすと、Unity上のカーソルが追従します。
    *   手を握る動作をすることで、ボタンのクリックやアイテムの掴み・スクロール操作が可能です。

## ライセンス・その他
本プロジェクトは卒業研究の一部として作成されたものです。
