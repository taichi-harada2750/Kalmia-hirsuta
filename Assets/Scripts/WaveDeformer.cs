using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class WaveDeformer : MonoBehaviour {
    public float amplitude1 = 0.2f;    // 揺れ幅1
    public float frequency1 = 1f;      // 周波数1
    public float speed1 = 1f;          // 速度1

    public float amplitude2 = 0.1f;    // 揺れ幅2
    public float frequency2 = 2f;      // 周波数2
    public float speed2 = 0.5f;        // 速度2

    private Mesh mesh;
    private Vector3[] baseVertices;

    void Start() {
        mesh = GetComponent<MeshFilter>().mesh;
        baseVertices = mesh.vertices;
    }

    void Update() {
        Vector3[] verts = new Vector3[baseVertices.Length];

        for (int i = 0; i < verts.Length; i++) {
            Vector3 v = baseVertices[i];
            float wave =
                Mathf.Sin(Time.time * speed1 + v.x * frequency1) * amplitude1 +
                Mathf.Sin(Time.time * speed2 + v.x * frequency2) * amplitude2;
            v.y += wave;
            verts[i] = v;
        }

        mesh.vertices = verts;
        mesh.RecalculateNormals();
    }
}
