using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MeshFilter))]
public class DrawNormals : Editor
{
    void OnSceneGUI()
    {
        MeshFilter meshFilter = (MeshFilter)target;
        if (meshFilter == null || meshFilter.sharedMesh == null) return;

        Mesh mesh = meshFilter.sharedMesh;
        Transform transform = meshFilter.transform;

        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;

        Handles.color = Color.red;

        for (int i = 0; i < vertices.Length; i++)
        {
            Vector3 worldPos = transform.TransformPoint(vertices[i]);
            Vector3 worldNormal = transform.TransformDirection(normals[i]);
            Handles.DrawLine(worldPos, worldPos + worldNormal * 200.0f);
        }
    }
}
