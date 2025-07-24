using UnityEngine;

public class ClassificationZone : MonoBehaviour
{
    public enum AcceptedType { Sphere, Cube }
    public AcceptedType acceptedType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Sphere") || other.CompareTag("Cube"))
        {
            bool isCorrect =
                (acceptedType == AcceptedType.Sphere && other.CompareTag("Sphere")) ||
                (acceptedType == AcceptedType.Cube && other.CompareTag("Cube"));

            // スコアを加算（SortGameManagerに依存）
            SortGameManager.Instance.AddScore(isCorrect);

            // オブジェクトを削除
            Destroy(other.gameObject);
        }
    }
}