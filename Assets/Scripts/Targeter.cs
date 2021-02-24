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
        if (target && selectable && selectable.IsSelected)
        {
            ShowMarker();
        } else
        {
            ClearMarker();
        }
    }

    public void SetTarget(Targetable targetable)
    {
        target = targetable.transform;
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
        marker.transform.localScale = new Vector3(target.GetComponent<Collider>().bounds.extents.magnitude*2, 1, target.GetComponent<Collider>().bounds.extents.magnitude * 2);
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
