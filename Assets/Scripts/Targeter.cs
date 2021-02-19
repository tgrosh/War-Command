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

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        
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
        marker = Instantiate(targetMarkerPrefab, new Vector3(target.position.x, 0, target.position.z), Quaternion.identity);
        marker.GetComponentInChildren<MeshRenderer>().material.color = Color.green;
        marker.transform.localScale = Vector3.one * target.GetComponent<Collider>().bounds.extents.magnitude;
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
