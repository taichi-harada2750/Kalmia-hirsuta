using UnityEngine;

public class DualAimShooter : MonoBehaviour
{
    [Header("ロックオン判定距離")]
    public float lockThreshold = 1.0f;

    [Header("破壊対象のレイヤー")]
    public LayerMask targetLayer;

    void Update()
    {
        Vector3 leftPalm = PalmDataManager.LeftPalm;
        Vector3 rightPalm = PalmDataManager.RightPalm;

        if (PalmDataManager.LeftGrabbing && PalmDataManager.RightGrabbing)
        {
            Collider[] nearbyTargets = Physics.OverlapSphere(leftPalm, lockThreshold, targetLayer);
            foreach (Collider col in nearbyTargets)
            {
                Vector3 targetPos = col.transform.position;  // ← ここで定義している

                Debug.Log($"L:{leftPalm}, R:{rightPalm}, Target:{targetPos}");

                if (Vector3.Distance(rightPalm, targetPos) < lockThreshold)
                {
                    TargetObject target = col.GetComponent<TargetObject>();
                    if (target != null)
                    {
                        target.DestroyWithEffect();
                        SortGameManager.Instance.AddScore(true);
                        break;
                    }
                }
            }
        }
    }

}
