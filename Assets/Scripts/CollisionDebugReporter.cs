using UnityEngine;

public class CollisionDebugReporter : MonoBehaviour
{
    void Start()
    {
        Collider col = GetComponent<Collider>();
        Rigidbody rb = GetComponent<Rigidbody>();

        Debug.Log($"[CollisionDebug] [{gameObject.name}] Start()");
        Debug.Log($" - Collider: {(col != null ? col.GetType().Name : "None")} | Trigger: {col?.isTrigger}");
        Debug.Log($" - Rigidbody: {(rb != null ? "Yes" : "None")} | isKinematic: {rb?.isKinematic}");
    }

    void OnCollisionEnter(Collision collision)
    {
        Debug.Log($"[CollisionDebug] OnCollisionEnter with: {collision.gameObject.name}");
    }

    void OnCollisionStay(Collision collision)
    {
        Debug.Log($"[CollisionDebug] OnCollisionStay with: {collision.gameObject.name}");
        
    }

    void OnCollisionExit(Collision collision)
    {
        Debug.Log($"[CollisionDebug] OnCollisionExit with: {collision.gameObject.name}");
    }

    void FixedUpdate()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 0.05f);
        Debug.Log($"[CollisionDebug] OverlapSphere hit count: {hits.Length}");
        foreach (var hit in hits)
        {
            Debug.Log($"  → {hit.name} (Layer: {hit.gameObject.layer})");
        }
    }
}
