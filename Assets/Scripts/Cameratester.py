import cv2

def list_cameras(max_index=10):
    print("🔍 カメラ一覧を確認中...")
    for i in range(max_index):
        cap = cv2.VideoCapture(i)
        if cap.read()[0]:
            print(f"✅ カメラ {i} が使用可能")
            cap.release()
        else:
            print(f"❌ カメラ {i} は使用不可")
    print("終了")

list_cameras()
