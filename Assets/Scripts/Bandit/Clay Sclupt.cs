using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class ClaySculpt : MonoBehaviour
{
    public float deformationRadius = 0.5f;
    public float grabStrength = 0.05f;

    private Mesh deformingMesh;
    private Vector3[] originalVertices;
    private Vector3[] displacedVertices;

    void Start()
    {
        deformingMesh = GetComponent<MeshFilter>().mesh;
        originalVertices = deformingMesh.vertices;
        displacedVertices = new Vector3[originalVertices.Length];
        for (int i = 0; i < originalVertices.Length; i++)
            displacedVertices[i] = originalVertices[i];
    }

    void Update()
    {
        ApplyDeformation(PalmDataManager.LeftPalm, PalmDataManager.LeftGrabbing);
        ApplyDeformation(PalmDataManager.RightPalm, PalmDataManager.RightGrabbing);

        deformingMesh.vertices = displacedVertices;
        deformingMesh.RecalculateNormals();
    }

    void ApplyDeformation(Vector3 palmPosition, bool isGrabbing)
    {
        Vector3 localPalm = transform.InverseTransformPoint(palmPosition);

        for (int i = 0; i < displacedVertices.Length; i++)
        {
            Vector3 pointToPalm = localPalm - displacedVertices[i];
            float distance = pointToPalm.magnitude;

            if (distance < deformationRadius)
            {
                float force = (deformationRadius - distance) / deformationRadius;
                if (isGrabbing)
                {
                    displacedVertices[i] += pointToPalm.normalized * force * grabStrength;
                }
                else
                {
                    displacedVertices[i] += pointToPalm.normalized * force * (grabStrength * 0.25f); // なでるだけでも少し変形
                }
            }
        }
    }
}
