using UnityEngine;

public class RingUIController : MonoBehaviour
{
    public RectTransform[] appIcons;
    public float radius = 200f; // UIの円半径（Canvas上の単位）

    void Start()
    {
        int count = appIcons.Length;
        for (int i = 0; i < count; i++)
        {
            float angle = i * Mathf.PI * 2 / count;
            float x = Mathf.Cos(angle) * radius;
            float y = Mathf.Sin(angle) * radius;

            appIcons[i].anchoredPosition = new Vector2(x, y);
        }
    }
}
