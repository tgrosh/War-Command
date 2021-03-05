using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Mover : NetworkBehaviour
{
    public GameObject locationMarkerPrefab;
    public bool moveComplete;
    public bool showPath;
    public NavMeshAgent agent;
    public float lastPositionSensitivity = .5f; //how many seconds do i need to be at the same position before deciding i have stopped

    GameObject marker; 
    LineRenderer pathRenderer;
    Vector3 currentTargetPosition;
    Transform currentTargetTransform;
    Animator animator;
    Vector3 lastPosition;
    float currentLastPositionTime;

    ActionQueue queue;
    Action currentAction;

    // Start is called before the first frame update
    void Start()
    {
        lastPosition = transform.position;
        moveComplete = true;
        agent = GetComponent<NavMeshAgent>();
        pathRenderer = GetComponent<LineRenderer>();
        animator = GetComponent<Animator>();
        queue = GetComponent<ActionQueue>();
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Peek() != null && queue.Peek().actionType == ActionType.Move)
        {
            currentAction = queue.Peek();
        } else
        {
            currentAction = null;
        }

        if (currentAction != null)
        {
            SetDestination(currentAction.actionPosition);
            ShowDestinationMarker();
        }

        if (!moveComplete && agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
        {
            ClearDestination();
            if (currentAction != null)
            {
                queue.Next();
            }
        }

        if (animator)
        {
            if (lastPosition != transform.position)
            {
                //we are moving, show animation
                animator.SetTrigger("move");
                animator.ResetTrigger("idle");
            } else
            {
                if (currentLastPositionTime > lastPositionSensitivity)
                {
                    //we are not moving
                    animator.SetTrigger("idle");
                    animator.ResetTrigger("move");
                    currentLastPositionTime = 0f;
                }
                currentLastPositionTime += Time.deltaTime;
            }
            lastPosition = transform.position;
        }

        if (hasAuthority && showPath) ShowPath();
    }

    void OnDestroy() {
        if (marker)
        {
            Destroy(marker);
        }
    }

    public void SetDestination(Vector3 targetPosition, float range = .1f, int navLayerMask = NavMesh.AllAreas)
    {
        if (currentTargetPosition == targetPosition) return;

        ClearDestination();
        agent.SetDestination(targetPosition);
        agent.stoppingDistance = range;
        currentTargetPosition = targetPosition;
        moveComplete = false;
    }

    public void SetDestination(Transform targetTransform)
    {
        if (currentTargetTransform == targetTransform) return;

        SetDestination(GetBounds(targetTransform).ClosestPoint(transform.position));
    }

    Bounds GetBounds(Transform targetTransform)
    {
        Bounds combinedBounds = new Bounds(targetTransform.position, Vector3.zero);
        foreach (Renderer renderer in targetTransform.GetComponentsInChildren<Renderer>())
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        return combinedBounds;
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

}
