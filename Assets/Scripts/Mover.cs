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
        agent.destination = targetPosition;
    }

    public void ShowDestinationMarker(Vector3 markerPosition)
    {
        if (marker)
        {
            Destroy(marker);
        }
        marker = Instantiate(locationMarkerPrefab, markerPosition + (Vector3.up * .1f), Quaternion.Euler(Vector3.right * 90));
    }
}
