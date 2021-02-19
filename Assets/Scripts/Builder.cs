using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public float buildRange;

    Mover mover;
    ToolbarAction currentBuildAction;
    Buildable currentBuildable;

    bool atBuildTarget;

    private void Start()
    {
        mover = GetComponent<Mover>();
    }

    private void Update()
    {
        atBuildTarget = mover.moveComplete && currentBuildable && Vector3.Distance(transform.position, currentBuildable.transform.position) < buildRange;

        if (atBuildTarget)
        {
            //start the build
            mover.ClearDestination();
            StartBuild();
        }
    }

    public void Build(Buildable buildable, ToolbarAction buildAction)
    {
        currentBuildAction = buildAction;
        currentBuildable = buildable;

        // goto buildable location
        mover.SetDestination(currentBuildable.transform.position);
    }

    void StartBuild()
    {
        if (ResourceBank.Withdraw(currentBuildAction.cost))
        {
            currentBuildable.GetComponent<Buildable>().Build();
            currentBuildAction = null;
            currentBuildable = null;
        }
    }

}
