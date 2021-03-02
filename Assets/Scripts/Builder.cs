using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public float buildRange;

    Mover mover;
    Targeter targeter;
    ToolbarAction currentBuildAction;
    Buildable buildTarget;

    bool atBuildTarget;    

    private void Start()
    {
        mover = GetComponent<Mover>();
        targeter = GetComponent<Targeter>();
    }

    private void Update()
    {
        if (targeter.target)
        {
            buildTarget = targeter.target.GetComponent<Buildable>();
        }
        else
        {
            if (buildTarget && buildTarget.currentBuildState == BuildState.PendingBuild)
            {
                buildTarget.CancelBuild();
            }
            buildTarget = null;
            currentBuildAction = null;
        }

        atBuildTarget = mover.moveComplete && buildTarget && Vector3.Distance(transform.position, buildTarget.transform.position) < buildRange;

        if (buildTarget && !atBuildTarget)
        {
            //move to build target
            mover.SetDestination(buildTarget.transform.position);
        }

        if (buildTarget && atBuildTarget)
        {
            //start the build
            mover.ClearDestination();
            StartBuild();
        }
    }

    public void Build(Buildable buildable, ToolbarAction buildAction)
    {
        currentBuildAction = buildAction;

        GetComponent<Targeter>().SetTarget(buildable.GetComponent<Targetable>());
    }

    void StartBuild()
    {
        transform.LookAt(new Vector3(buildTarget.transform.position.x, transform.position.y, buildTarget.transform.position.z));

        if (ResourceBank.Withdraw(currentBuildAction.cost))
        {
            buildTarget.GetComponent<Buildable>().Build();
            targeter.ClearTarget();
        }
    }

}
