import cv2
import mediapipe as mp
from pythonosc import udp_client
import threading
import time

# --- 設定 ---
camera_index = 0          # 使用するカメラ番号
OSC_IP = "127.0.0.1"
OSC_PORT = 9000
# 解像度は必ず16:9を維持すること。4:3にすると正規化座標(palm_x/y)の視野が
# アプリ側の16:9キャンバスと合わず、操作できない領域が発生する。
# 16:9の低解像度候補: 640x360(最軽量) / 960x540 / 1280x720
CAM_WIDTH = 1280
CAM_HEIGHT = 720
CAM_FPS = 30
SHOW_PREVIEW = True       # 本番展示ではFalse推奨（imshow/waitKeyのコストを削減）

# --- MediaPipe Hands 初期化 ---
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,           # 両手対応
    model_complexity=0,        # 0=軽量モデル。掌中心の精度差はほぼ無く、処理は大幅に高速
    min_detection_confidence=0.7,
    min_tracking_confidence=0.5)   # 低めにして追跡状態を維持し、重い再検出の頻度＝遅延スパイクを減らす

client = udp_client.SimpleUDPClient(OSC_IP, OSC_PORT)


# --- 常に最新フレームだけを保持するスレッド化キャプチャ ---
# OpenCVの内部バッファに古いフレームが溜まって遅延が累積するのを防ぐ。
class FreshestFrame:
    def __init__(self, cap):
        self.cap = cap
        self.lock = threading.Lock()
        self.frame = None
        self.seq = 0            # フレーム更新のたびに増加（同一フレームの再処理を防ぐ）
        self.cap_fps = 0.0      # カメラが実際に供給しているFPS（＝取得律速の有無を判定）
        self.running = True
        self.thread = threading.Thread(target=self._loop, daemon=True)
        self.thread.start()

    def _loop(self):
        last = time.perf_counter()
        while self.running:
            ret, f = self.cap.read()
            if not ret:
                time.sleep(0.001)
                continue
            now = time.perf_counter()
            d = now - last
            last = now
            if d > 0:
                inst = 1.0 / d
                self.cap_fps = inst if self.cap_fps == 0.0 else self.cap_fps * 0.9 + inst * 0.1
            with self.lock:
                self.frame = f
                self.seq += 1

    def read(self):
        with self.lock:
            if self.frame is None:
                return None, self.seq
            return self.frame.copy(), self.seq

    def release(self):
        self.running = False
        self.thread.join(timeout=1.0)


# --- カメラ準備 ---
def decode_fourcc(v):
    """CAP_PROP_FOURCCの数値を'MJPG'等の文字列に変換。"""
    v = int(v)
    return "".join([chr((v >> 8 * i) & 0xFF) for i in range(4)])


# 既定バックエンド（Windowsでは多くの場合MSMF）はC270の720pで自動的にMJPG@30fpsを選ぶ。
# 以前 DirectShow(CAP_DSHOW) を強制したところ YUY2 にフォールバックして7.5fpsに落ちたため、
# 既定バックエンドに戻す（＝更新前の30fps構成）。
cap = cv2.VideoCapture(camera_index)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, CAM_WIDTH)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, CAM_HEIGHT)
cap.set(cv2.CAP_PROP_FPS, CAM_FPS)

if not cap.isOpened():
    raise RuntimeError(f"❌ カメラ {camera_index} を開けません。camera_index を確認してください。")

fresh = FreshestFrame(cap)
actual_w = int(cap.get(cv2.CAP_PROP_FRAME_WIDTH))
actual_h = int(cap.get(cv2.CAP_PROP_FRAME_HEIGHT))
aspect = actual_w / actual_h if actual_h else 0
print(f"🎥 カメラ {camera_index} を使用中。'q'キーで終了します。")
reported_fps = cap.get(cv2.CAP_PROP_FPS)
actual_fourcc = decode_fourcc(cap.get(cv2.CAP_PROP_FOURCC))
print(f"📐 実解像度: {actual_w}x{actual_h} (アスペクト比 {aspect:.3f}, 16:9={16/9:.3f})")
print(f"🎞️  カメラ報告FPS: {reported_fps:.0f} / フォーマット: {actual_fourcc}")
if actual_fourcc != "MJPG":
    print("⚠️  MJPGではありません。C270の720pはYUY2だと約7.5fpsに制限されます。")
    print("    → 30fps出ない場合は、他アプリがカメラ設定を変えていないか確認してください。")
if abs(aspect - 16 / 9) > 0.01:
    print("⚠️  16:9ではありません。操作不能領域が出る可能性あり。解像度設定を確認してください。")


# --- 掴みポーズ判定 ---
def is_grabbing_pose(landmarks):
    def folded(tip_id, pip_id):
        return landmarks[tip_id].y > landmarks[pip_id].y
    return (
        folded(8, 6) and
        folded(12, 10) and
        folded(16, 14) and
        folded(20, 18)
    )


# --- メインループ ---
last_seq = -1
fps = 0.0                       # 平滑化した処理FPS
last_frame_time = time.perf_counter()
last_print_time = last_frame_time
try:
    while True:
        frame, seq = fresh.read()
        if frame is None or seq == last_seq:
            # まだ新しいフレームが来ていない場合はCPUを回しすぎない
            time.sleep(0.001)
            continue
        last_seq = seq

        # --- 処理FPS計測（指数移動平均で平滑化）---
        now = time.perf_counter()
        dt = now - last_frame_time
        last_frame_time = now
        if dt > 0:
            inst_fps = 1.0 / dt
            fps = inst_fps if fps == 0.0 else fps * 0.9 + inst_fps * 0.1
        if now - last_print_time >= 1.0:   # 1秒ごとにコンソール出力
            # キャプチャFPS: カメラの供給レート / 処理FPS: MediaPipe処理後の送信レート
            print(f"⚡ キャプチャFPS: {fresh.cap_fps:.1f} | 処理FPS: {fps:.1f}")
            last_print_time = now

        rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
        results = hands.process(rgb_frame)

        output_frame = frame if SHOW_PREVIEW else None

        if results.multi_hand_landmarks and results.multi_handedness:
            for i, hand_landmarks in enumerate(results.multi_hand_landmarks):
                handedness = results.multi_handedness[i].classification[0].label  # 'Left' or 'Right'
                landmarks = hand_landmarks.landmark

                wrist = landmarks[0]
                middle_base = landmarks[9]
                palm_x = (wrist.x + middle_base.x) / 2
                palm_y = (wrist.y + middle_base.y) / 2
                grabbing = is_grabbing_pose(landmarks)

                # アドレス分岐（平滑化はUnity受信側のOne-Euroフィルタで実施）
                address = "/hand/left_palm" if handedness == "Left" else "/hand/right_palm"
                client.send_message(address, [palm_x, palm_y, float(grabbing)])

                # 可視化
                if SHOW_PREVIEW:
                    h, w, _ = output_frame.shape
                    cx, cy = int(palm_x * w), int(palm_y * h)
                    color = (255, 0, 0) if handedness == "Left" else (0, 255, 255)
                    if grabbing:
                        color = (0, 255, 0)
                    cv2.circle(output_frame, (cx, cy), 14, color, -1)

        if SHOW_PREVIEW:
            cv2.putText(output_frame, f"cap:{fresh.cap_fps:.0f} proc:{fps:.0f}  {actual_w}x{actual_h}",
                        (10, 30), cv2.FONT_HERSHEY_SIMPLEX, 0.8, (0, 255, 0), 2)
            cv2.imshow("MediaPipe Dual Hand Tracker", output_frame)
            if cv2.waitKey(1) & 0xFF == ord('q'):
                break
finally:
    fresh.release()
    cap.release()
    cv2.destroyAllWindows()
