using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Moveable : MonoBehaviour
{
    public GameObject locationMarkerPrefab;
    public GameObject targetMarkerPrefab;

    NavMeshAgent agent;
    GameObject marker;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (marker && agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
        {
            Destroy(marker);
        }
    }

    public void SetDestinationTarget(Transform targetTransform)
    {
        agent.destination = targetTransform.position;
        ShowMarker(targetMarkerPrefab, new Vector3(targetTransform.position.x, .55f, targetTransform.position.z));
        SetTarget(targetTransform);
    }

    void SetTarget(Transform targetTransform)
    {
        Targeter targeter = GetComponent<Targeter>();

        if (targeter)
        {
            targeter.target = targetTransform;
        }
    }

    public void SetDestinationPosition(Vector3 targetPosition)
    {
        agent.destination = targetPosition;
        ShowMarker(locationMarkerPrefab, targetPosition);
        SetTarget(null);
    }

    void ShowMarker(GameObject prefab, Vector3 markerPosition)
    {
        if (marker)
        {
            Destroy(marker);
        }
        marker = Instantiate(prefab, markerPosition + (Vector3.up * .1f), Quaternion.Euler(Vector3.right * 90));
    }
}
