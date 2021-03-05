using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public float buildRange;

    Mover mover;
    ActionQueue queue;
    Action currentAction;
    Buildable buildTarget;

    bool atBuildTarget;    

    private void Start()
    {
        mover = GetComponent<Mover>();
        queue = GetComponent<ActionQueue>();
    }

    private void Update()
    {
        if (queue.Peek() != null && queue.Peek().actionType == ActionType.Build)
        {
            currentAction = queue.Peek();
        }
        else
        {
            currentAction = null;
        }

        if (currentAction != null)
        {
            buildTarget = currentAction.actionTarget.GetComponent<Buildable>();
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
        queue.Clear();
        queue.Add(new Action(ActionType.Build, buildable.gameObject));
    }

    void StartBuild()
    {
        transform.LookAt(new Vector3(buildTarget.transform.position.x, transform.position.y, buildTarget.transform.position.z));

        if (ResourceBank.Withdraw(buildTarget.ironCost, buildTarget.oilCost))
        {
            buildTarget.GetComponent<Buildable>().Build();
            queue.Next();
        }
    }

}
