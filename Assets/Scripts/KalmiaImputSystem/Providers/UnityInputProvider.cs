using UnityEngine;
using System;
using KIS.Core;

namespace KIS.Providers
{
    /// <summary>
    /// マウスを第3の手としてKISに登録するためのプロバイダー
    /// </summary>
    public class UnityInputProvider : MonoBehaviour, IInputProvider
    {
        public event Action<HandData> OnLeftHandUpdated;
        public event Action<HandData> OnRightHandUpdated;
        public event Action<HandData> OnMouseUpdated;

        private Vector3 prevMousePos;

        [Tooltip("マウスポインタの代わりとなる画面上のカーソルオブジェクト（SphereCollider等を持たせる）\n※名前には 'Mouse' を含めてください。")]
        public Transform mouseCursorTransform;

        [Tooltip("UIが存在する仮想的なZ平面（現在Radius9で当てているZ=0地点）")]
        public float uiZPosition = 0f;

        public void Initialize()
        {
            if (mouseCursorTransform == null)
            {
                GameObject obj = GameObject.Find("MouseCursor");
                if (obj != null) mouseCursorTransform = obj.transform;
            }

            if (mouseCursorTransform != null)
            {
                if (mouseCursorTransform.GetComponent<Collider>() == null)
                {
                    var col = mouseCursorTransform.gameObject.AddComponent<SphereCollider>();
                    col.radius = 0.5f;
                    col.isTrigger = true;
                    Debug.LogWarning("[KIS] MouseCursorにコライダーがなかったため自動追加しました。");
                }
                if (mouseCursorTransform.GetComponent<Rigidbody>() == null)
                {
                    var rb = mouseCursorTransform.gameObject.AddComponent<Rigidbody>();
                    rb.isKinematic = true;
                    rb.useGravity = false;
                }
            }
            Debug.Log("[KIS] UnityInputProvider (Mouse) Initialized.");
        }

        public void UpdateProvider()
        {
            Camera cam = Camera.main;
            if (cam == null) cam = FindObjectOfType<Camera>();
            if (cam == null) return; // それでも無い場合はスキップ

            // メインカメラの奥方向（Z）への距離を計算してScreenToWorld
            float depth = Mathf.Abs(cam.transform.position.z - uiZPosition);
            Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth);

            Vector3 worldPos = cam.ScreenToWorldPoint(mouseScreenPos);

            // 既存仕様に合わせてZ=0に固定
            worldPos.z = uiZPosition;

            // もしカーソル用のオブジェクトが指定されていれば、そこに座標を反映させる
            if (mouseCursorTransform != null)
            {
                mouseCursorTransform.position = worldPos;
            }

            bool isGrabbing = Input.GetMouseButton(0); // 左クリックでGrab判定

            Vector3 velocity = (Time.deltaTime > 0) ? (worldPos - prevMousePos) / Time.deltaTime : Vector3.zero;

            HandData data = new HandData(worldPos, velocity, isGrabbing,
                Time.time, KISHand.Mouse, true);
            OnMouseUpdated?.Invoke(data);
            
            prevMousePos = worldPos;
        }
    }
}
