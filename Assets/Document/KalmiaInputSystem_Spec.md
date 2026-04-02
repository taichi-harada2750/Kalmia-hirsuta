---
aliases: 
tags: 
date:
---

# Kalmia Input System（KIS）設計概要 - v1.1
*ver.2025.10 — Kalmia hirsuta / NUI統合仕様対応版*

---

## 1. 概要
**KIS (Kalmia Input System)** は、Mediapipe・Kinectなど異なる入力デバイスを  
Kalmia内部の**ユニバーサル入力規格**に統一する中間層である。  
これにより、非接触操作を**Unity標準UIおよびOpenXR UI**で  
同一のインターフェースとして扱うことが可能になる。

---

## 2. 目的
- **デバイス非依存化**：Mediapipe / Kinect / Mouse / Keyboardなどを統一形式で扱う  
- **規格統一**：Unity標準UIおよびOpenXR規格と互換性を持つ「Universal Input Spec」を実現  
- **拡張基盤**：将来的なXR UI Kitやバーチャルキーボード実装に対応可能な拡張性を確保  

---

## 3. システム構造

```

[入力層]  
├─ MediapipeProvider  
├─ KinectProvider  
├─ UnityInputProvider（キーマウ入力）  
└─ OpenXRProvider（XRデバイス）

```
    ↓ HandData(position, velocity, isGrabbing)
```

[中核層 - KIS Core]  
├─ GestureRecognizer // 動作検出  
├─ IntentInterpreter // 行為→意図変換  
├─ PointerManager // カーソル管理  
├─ EventBus // Intent発火  
└─ OutputAdapterManager // 出力経路制御

```
    ↓
```

[出力層]  
├─ UnityUIOutputAdapter → Canvas / UI Toolkit（2D UI）  
├─ XROutputAdapter → OpenXR Actions（3D / XR UI）  
└─ SystemInputOutput → OSエミュレーション（任意）

```
    ↓
```

[UI層]  
├─ Kalmia独自UI（RingUI, ArcUI, SortSphereなど）  
├─ Unity標準UI（Canvas, Button, ScrollRect）  
└─ OpenXR UI（MRTK3, XR Interaction Toolkit）

````

---

## 4. ユニバーサル入力規格（Universal Input Spec）

| 要件          | 内容                                                   |
| ----------- | ---------------------------------------------------- |
| **座標系**     | Unityワールド空間（m単位, OpenXR準拠）                           |
| **入力粒度**    | Pointer（位置）＋Intent（意味）                               |
| **イベント形式**  | Unity EventSystem準拠（`PointerClick`, `PointerEnter`等） |
| **アクション構造** | Intent ≒ InputAction（OpenXR Actionに対応）               |
| **デバイス中立性** | Providerによってデバイス依存を抽象化                               |

---

## 5. Intent定義（共通入力フォーマット）

| Intent名 | 検出条件 | 出力用途（Unity / OpenXR） |
|-----------|-----------|-----------------------------|
| `GrabIntent` | isGrabbing == true | Click / Select |
| `ReleaseIntent` | Grab → Open | PointerUp / ValueOff |
| `HoverIntent` | 距離閾値内で静止 | Hover / AimPose |
| `GrabSwipeIntent` | isGrabbing && velocity > threshold | Drag / ThumbstickY |
| `HoldIntent` | Grab継続時間 > holdTime | LongPress / MenuOpen |

---

## 6. 出力仕様（Output Adapter）

### 出力制御
KISは出力先を選択可能な設計とする：

```csharp
public enum OutputTarget {
    UnityUI,
    OpenXR,
    Both
}
````

### 出力方向

|Adapter名|出力対象|通信形式|備考|
|---|---|---|---|
|**UnityUIOutput**|Unity EventSystem|`PointerEventData` / `ExecuteEvents.Execute()`|標準Canvas/UI Toolkit対応|
|**XROutputAdapter**|OpenXR Action|`InputAction.Trigger()`|MRTK3 / XR UI Kit対応|
|**SystemInputOutput**|OSレイヤー|Windows API `SendInput()`|任意（展示デバッグ用）|

> 双方向同期は行わず、KIS → Unity / KIS → XR の片方向出力に限定。  
> これによりFocus競合・座標系不整合を回避。

---

## 7. Intentと出力先の対応表

|Intent|Unity出力|OpenXR出力|共通用途|
|---|---|---|---|
|GrabIntent|PointerClick|`/user/hand/right/input/select/click`|決定・選択|
|ReleaseIntent|PointerUp|`/user/hand/right/input/select/value`|解放|
|HoverIntent|PointerEnter/Exit|`/user/hand/right/input/aim/pose`|注視|
|GrabSwipeIntent|Scroll / Drag|`/user/hand/right/input/thumbstick/y`|スクロール|
|HoldIntent|PointerDown (長押し)|`/user/hand/right/input/menu/click`|長押し・メニュー操作|

---

## 8. Pointer / カーソル挙動

|モード|操作源|出力方式|表示方法|
|---|---|---|---|
|**UnityUIモード**|Mediapipe / Mouse|Canvas座標（2D）|Overlayカーソル|
|**OpenXRモード**|Kinect / XR Hand|ワールド座標（3D）|3D Cursor (Raycast)|
|**Mixedモード**|両方|DualAdapter出力|開発・デバッグ用|

---

## 9. フィルタリング・安定化

|処理|内容|
|---|---|
|**デッドゾーン**|微小な位置変化（<0.02m）を静止とみなす|
|**スムージング**|`lerp(prevPos, newPos, 0.3f)` による平滑化|
|**意図確認**|Hover中にGrabでのみIntent発火|
|**速度閾値**|Swipe閾値：0.25 m/s（初期値）|
|**フレーム欠落補間**|Mediapipe消失時に過去2フレーム補完|

---

## 10. 出力統合の開発方針（2025下期）

|フェーズ|内容|対応範囲|
|---|---|---|
|Phase 1|UnityUIOutput整備|Grab→ClickでCanvas動作確認|
|Phase 2|XROutputAdapter実装|OpenXR Action発火確認|
|Phase 3|`OutputTarget`切替実装|Unity／XR出力の選択対応|
|Phase 4|Bothモード検証|両出力同時動作確認|
|Phase 5|UI統合テスト|Kalmia UI＋Unity UI＋XR UIの連携確認|

---

## 11. 期待される成果

| 項目        | 効果                                   |
| --------- | ------------------------------------ |
| **統一入力層** | KIS Intentを介してすべてのUI操作を一元化           |
| **互換性確保** | Unity標準UI / OpenXR UI 両対応            |
| **実験容易性** | デスクトップ・XR展示を設定で切替可能                  |
| **実装効率**  | OutputAdapter差替えのみで環境変更対応            |
| **拡張余地**  | 将来的なVirtual Keyboard / Voice Input連携 |

---

## 12. 開発構成（最終構造）

```
KIS/
 ├── Providers/
 │    ├── MediapipeProvider.cs
 │    ├── KinectProvider.cs
 │    ├── UnityInputProvider.cs
 │    └── OpenXRProvider.cs
 ├── Core/
 │    ├── GestureRecognizer.cs
 │    ├── IntentInterpreter.cs
 │    ├── PointerManager.cs
 │    └── EventBus.cs
 ├── Output/
 │    ├── UnityUIOutputAdapter.cs
 │    ├── XROutputAdapter.cs
 │    └── SystemInputOutput.cs
 ├── Debug/
 │    ├── KISDebugOverlay.cs
 │    └── KISLogger.cs
 └── Interfaces/
      ├── IInputProvider.cs
      ├── IOutputAdapter.cs
      └── IKISIntentListener.cs
```

---

## 13. 将来拡張（予定）

- **Virtual Keyboard統合**：Intentを直接UI Toolkit入力へ転送
    
- **XR UI Kit対応強化**：OpenXR Action Pathを標準化
    
- **Voice / Gaze Provider**：非物理入力をIntentに統合
    
- **NetworkAdapter**：外部ツール（TouchDesigner等）との橋渡し
    

---

## 要約

|要点|内容|
|---|---|
|**KISの役割**|異種デバイス入力を統一Intent化し、Unity／OpenXR UIにアクセス可能な中間層|
|**基本方針**|双方向同期は行わず、片方向アクセスに限定|
|**最小互換**|Unity標準UI（EventSystem）|
|**拡張互換**|OpenXR（Action Pathベース）|
|**運用形態**|Unity展示・XR実験どちらにも利用可能|
|**目標**|“KISを通じて両UI規格にアクセスできるUniversal Input Hub” を確立|
