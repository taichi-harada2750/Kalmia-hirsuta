using UnityEngine;

public class UVScroller : MonoBehaviour {
    public float scrollSpeed = 0.02f;
    Renderer rend;
    Vector2 offset;

    void Start() {
        rend = GetComponent<Renderer>();
    }

    void Update() {
        offset.x += scrollSpeed * Time.deltaTime;
        rend.material.SetTextureOffset("_MainTex", offset);
    }
}
