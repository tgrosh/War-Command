using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : MonoBehaviour
{
    public Transform target;
    public GameObject targetMarkerPrefab;

    GameObject marker;

    void Update()
    {
        if (target == null)
        {
            ClearTarget();
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
        ShowMarker();
    }

    void ShowMarker()
    {
        if (marker)
        {
            Destroy(marker);
        }
        marker = Instantiate(targetMarkerPrefab, target.position + (Vector3.up * .1f), Quaternion.Euler(Vector3.right * 90));
    }

    public void ClearTarget()
    {
        target = null;
        if (marker)
        {
            Destroy(marker);
        }
    }
}
