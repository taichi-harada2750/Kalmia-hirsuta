using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KIS.Output;
using KIS.Core;


public class HoldTest : MonoBehaviour
{
    public UnityUIOutputAdapter kisOutputAdapter;

    private void OnEnable()
    {
        if (kisOutputAdapter != null)
        {
            kisOutputAdapter.OnHoldDetected += HandleHold;
        }
    }

    private void OnDisable()
    {
        if (kisOutputAdapter != null)
        {
            kisOutputAdapter.OnHoldDetected -= HandleHold;
        }
    }

    private void HandleHold (Vector3 position)
    {
        Debug.Log("Long press!");
    }

}
