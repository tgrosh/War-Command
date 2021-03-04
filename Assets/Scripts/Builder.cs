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
            if (buildTarget && buildTarget.currentBuildState != BuildState.PendingBuild)
            {
                buildTarget = null;
            }
        }
        else
        {
            buildTarget = null;
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

    public void Build(Buildable buildable)
    {
        GetComponent<Targeter>().SetTarget(buildable.GetComponent<Targetable>());
    }

    void StartBuild()
    {
        transform.LookAt(new Vector3(buildTarget.transform.position.x, transform.position.y, buildTarget.transform.position.z));

        if (ResourceBank.WithdrawIron(buildTarget.cost))
        {
            buildTarget.GetComponent<Buildable>().Build();
            targeter.ClearTarget();
        }
    }

}
