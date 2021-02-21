using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : MonoBehaviour
{
    public GameObject locationMarkerPrefab;
    public bool moveComplete;
    public bool showPath;
    public NavMeshAgent agent;

    GameObject marker; 
    LineRenderer pathRenderer;
    Vector3 currentTargetPosition;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        moveComplete = true;
        agent = GetComponent<NavMeshAgent>();
        pathRenderer = GetComponent<LineRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!moveComplete && agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
        {
            moveComplete = true;
            if (marker)
            {
                Destroy(marker);
            }
        }

        if (!moveComplete)
        {
            if (animator) animator.SetTrigger("move");
            if (animator) animator?.ResetTrigger("idle");
        }
        if (moveComplete)
        {
            if (animator) animator?.SetTrigger("idle");
            if (animator) animator?.ResetTrigger("move");
        }

        if (showPath) ShowPath();
    }

    public void SetDestination(Vector3 targetPosition, float range = 10f, int navLayerMask = NavMesh.AllAreas)
    {
        if (currentTargetPosition == targetPosition) return;

        NavMeshHit navHit;
        NavMesh.SamplePosition(targetPosition, out navHit, range, navLayerMask);
        
        if (navHit.hit) {
            ClearDestination();
            agent.destination = navHit.position;
            currentTargetPosition = navHit.position;
            moveComplete = false;
        }
    }

    public void SetDestinationRandom(Vector3 targetPosition, float range = 10f, int navLayerMask = NavMesh.AllAreas)
    {
        if (currentTargetPosition == targetPosition) return;
        
        ClearDestination();
        agent.destination = RandomNavmeshLocation(targetPosition, range);
        currentTargetPosition = targetPosition;
        moveComplete = false;
    }

    public void ClearDestination()
    {
        if (marker)
        {
            Destroy(marker);
        }
        agent.ResetPath();
        moveComplete = true;
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

    public Vector3 RandomNavmeshLocation(Vector3 targetPosition, float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += targetPosition;
        NavMeshHit hit;
        Vector3 finalPosition = targetPosition;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
