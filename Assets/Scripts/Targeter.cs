using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Targeter : MonoBehaviour
{
    public Transform target;
    public GameObject targetMarkerPrefab;

    NavMeshAgent agent;
    GameObject marker;
    Selectable selectable;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        selectable = GetComponent<Selectable>();
    }

    void Update()
    {
        if (target && selectable && selectable.isSelected)
        {
            ShowMarker();
        } else
        {
            ClearMarker();
        }
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    public void ShowMarker()
    {
        if (marker)
        {
            Destroy(marker);
        }

        if (!target) return;
        marker = Instantiate(targetMarkerPrefab, new Vector3(target.position.x, 0, target.position.z), Quaternion.identity);
        marker.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        marker.transform.localScale = Vector3.one * target.GetComponent<Collider>().bounds.extents.magnitude;
    }

    void ClearMarker()
    {
        if (marker)
        {
            Destroy(marker);
        }
    }

    public void ClearTarget()
    {
        target = null;
        ClearMarker();
    }
}
