using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    public GameObject locationMarkerPrefab;
    public bool showPath;

    NavMeshAgent agent;
    GameObject marker; 
    LineRenderer pathRenderer;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (marker && agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
        {
            Destroy(marker);
        }

        if (showPath) ShowPath();
    }

    public void SetDestination(Vector3 targetPosition, float range = 3f, int navLayerMask = NavMesh.AllAreas)
    {
        NavMeshHit navHit;
        NavMesh.SamplePosition(targetPosition, out navHit, range, navLayerMask);
        
        if (navHit.hit) {
            agent.ResetPath();
            agent.destination = navHit.position;
        }
    }

    public void ClearDestination()
    {
        if (marker)
        {
            Destroy(marker);
        }
    }

    public void ShowDestinationMarker()
    {
        if (marker)
        {
            Destroy(marker);
        }
        if (agent.destination != transform.position)
        {
            marker = Instantiate(locationMarkerPrefab, agent.destination, Quaternion.identity);
            marker.transform.localScale = Vector3.one * 2f;
        }
    }

    public void ShowPath()
    {
        NavMeshPath path = new NavMeshPath(); ;
        NavMesh.CalculatePath(transform.position, agent.destination, NavMesh.AllAreas, path);
        pathRenderer.positionCount = path.corners.Length;
        pathRenderer.SetPositions(path.corners);
    }
}
