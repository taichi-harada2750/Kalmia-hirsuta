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
            Debug.Log("[KIS] UnityInputProvider (Mouse) Initialized.");
        }

        public void UpdateProvider()
        {
            if (Camera.main == null) return;

            // メインカメラの奥方向（Z）への距離を計算してScreenToWorld
            float depth = Mathf.Abs(Camera.main.transform.position.z - uiZPosition);
            Vector3 mouseScreenPos = new Vector3(Input.mousePosition.x, Input.mousePosition.y, depth);

            Vector3 worldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

            // 既存仕様に合わせてZ=0に固定
            worldPos.z = uiZPosition;

            // もしカーソル用のオブジェクトが指定されていれば、そこに座標を反映させる
            if (mouseCursorTransform != null)
            {
                mouseCursorTransform.position = worldPos;
            }

            bool isGrabbing = Input.GetMouseButton(0); // 左クリックでGrab判定

            Vector3 velocity = (Time.deltaTime > 0) ? (worldPos - prevMousePos) / Time.deltaTime : Vector3.zero;

            HandData data = new HandData(worldPos, velocity, isGrabbing, Time.time);
            OnMouseUpdated?.Invoke(data);
            
            prevMousePos = worldPos;
        }
    }
}
