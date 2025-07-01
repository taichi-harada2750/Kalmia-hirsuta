import cv2
import mediapipe as mp
from pythonosc import udp_client
import math

# --- 設定 ---
camera_index = 1  # 使用するカメラ番号
OSC_IP = "127.0.0.1"
OSC_PORT = 9000

# --- MediaPipe Hands 初期化 ---
mp_hands = mp.solutions.hands
hands = mp_hands.Hands(
    static_image_mode=False,
    max_num_hands=2,  # 両手対応
    min_detection_confidence=0.7,
    min_tracking_confidence=0.7)

client = udp_client.SimpleUDPClient(OSC_IP, OSC_PORT)

# --- カメラ準備 ---
cap = cv2.VideoCapture(camera_index)
cap.set(cv2.CAP_PROP_FRAME_WIDTH, 1280)
cap.set(cv2.CAP_PROP_FRAME_HEIGHT, 720)

if not cap.isOpened():
    raise RuntimeError(f"❌ カメラ {camera_index} を開けません。")

print(f"🎥 カメラ {camera_index} を使用中。'q'キーで終了します。")

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
while cap.isOpened():
    ret, frame = cap.read()
    if not ret:
        print("フレーム取得失敗")
        break

    rgb_frame = cv2.cvtColor(frame, cv2.COLOR_BGR2RGB)
    results = hands.process(rgb_frame)
    output_frame = frame.copy()

    if results.multi_hand_landmarks and results.multi_handedness:
        for i, hand_landmarks in enumerate(results.multi_hand_landmarks):
            handedness = results.multi_handedness[i].classification[0].label  # 'Left' or 'Right'
            landmarks = hand_landmarks.landmark

            wrist = landmarks[0]
            middle_base = landmarks[9]
            palm_x = (wrist.x + middle_base.x) / 2
            palm_y = (wrist.y + middle_base.y) / 2
            grabbing = is_grabbing_pose(landmarks)

            # アドレス分岐
            address = "/hand/left_palm" if handedness == "Left" else "/hand/right_palm"
            client.send_message(address, [palm_x, palm_y, float(grabbing)])

            # 可視化
            h, w, _ = frame.shape
            cx, cy = int(palm_x * w), int(palm_y * h)
            color = (255, 0, 0) if handedness == "Left" else (0, 255, 255)
            if grabbing:
                color = (0, 255, 0)
            cv2.circle(output_frame, (cx, cy), 14, color, -1)

    cv2.imshow("MediaPipe Dual Hand Tracker", output_frame)
    if cv2.waitKey(1) & 0xFF == ord('q'):
        break

cap.release()
cv2.destroyAllWindows()
