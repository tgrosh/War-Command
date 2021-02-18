using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    public GameObject locationMarkerPrefab;

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

    public void SetDestinationPosition(Vector3 targetPosition)
    {
        agent.ResetPath();
        agent.destination = targetPosition;
    }

    public void ClearDestination()
    {
        if (marker)
        {
            Destroy(marker);
        }
    }

    public void ShowDestinationMarker(Vector3 markerPosition)
    {
        if (marker)
        {
            Destroy(marker);
        }
        marker = Instantiate(locationMarkerPrefab, markerPosition, Quaternion.identity);
        marker.transform.localScale = Vector3.one * 2f;
    }
}
