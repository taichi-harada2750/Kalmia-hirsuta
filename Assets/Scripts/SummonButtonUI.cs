using UnityEngine;

public class SummonButtonUI : MonoBehaviour
{
    public RectTransform summonButton;
    public GameObject ringUI;
    public float activationDistance = 15f; // ← 10〜20くらいでまず試す


    void Update()
    {
        Vector3 handPos = PalmDataManager.RightPalm;
        bool isGrabbing = PalmDataManager.RightGrabbing;
        Vector3 buttonPos = summonButton.position;

        float distance = Vector3.Distance(handPos, buttonPos);
        Vector3 buttonWorldPos = summonButton.TransformPoint(summonButton.rect.center);
        float Distance = Vector3.Distance(handPos, buttonWorldPos);

        Debug.Log($"[DEBUG] handPos: {handPos}, buttonPos: {buttonPos}, distance: {distance:F2}, grabbing: {isGrabbing}");
        if (distance < activationDistance)
        {
            Debug.LogWarning("距離条件クリア");
        }
        if (isGrabbing)
        {
            Debug.LogWarning("Grabbing条件クリア");
        }
        if (distance < activationDistance && isGrabbing)
        {
            Debug.LogWarning("両方の条件クリア → SetActiveへ");
        }


        if (distance < activationDistance && isGrabbing)
        {
            ringUI.SetActive(true);
            ringUI.transform.position = summonButton.position;
            Debug.Log("条件を満たしてリングUIを表示！");
        }


        if (Input.GetKeyDown(KeyCode.Space))
        {
            ringUI.SetActive(true);
            ringUI.transform.position = summonButton.position;
            Debug.Log("スペースキーでリングUIを表示！");
        }
    }


}
