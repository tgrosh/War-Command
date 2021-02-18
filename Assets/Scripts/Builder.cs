using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public List<BuildAction> buildMenuItems = new List<BuildAction>();
    public bool showPath;

    NavMeshAgent agent;
    LineRenderer pathRenderer;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        pathRenderer = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        if (showPath) ShowPath();
    }

    public void Select()
    {
        EventManager.Emit(EventManager.Events.RegisterBuilder, this);
    }

    public void Unselect()
    {
        EventManager.Emit(EventManager.Events.RegisterBuilder, null);
    }

    public void Build(Buildable buildable)
    {
        // goto buildable location
        NavMeshHit navMeshHit;
        if (NavMesh.SamplePosition(buildable.transform.position, out navMeshHit, buildable.currentActor.GetComponent<NavMeshObstacle>().radius * 1.25f, NavMesh.AllAreas))
        {
            agent.SetDestination(navMeshHit.position);
        }
        // when it gets there, call build on the buildable
    }

    public void ShowPath()
    {
        NavMeshPath path = new NavMeshPath(); ;
        NavMesh.CalculatePath(transform.position, agent.destination, NavMesh.AllAreas, path);
        pathRenderer.positionCount = path.corners.Length;
        pathRenderer.SetPositions(path.corners);
    }

}
