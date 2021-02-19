using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public List<BuildAction> buildMenuItems = new List<BuildAction>();
    public float buildRange;

    Mover mover;
    Selectable selectable;
    BuildAction currentBuildAction;
    Buildable currentBuildable;

    bool registered;
    bool atBuildTarget;

    private void Start()
    {
        mover = GetComponent<Mover>();
        selectable = GetComponent<Selectable>();
    }

    private void Update()
    {
        atBuildTarget = mover.moveComplete && currentBuildable && Vector3.Distance(transform.position, currentBuildable.transform.position) < buildRange;

        if (selectable && selectable.isSelected && !registered)
        {
            EventManager.Emit(EventManager.Events.RegisterBuilder, this);
            registered = true;
        }

        if (!selectable || (!selectable.isSelected && registered))
        {
            EventManager.Emit(EventManager.Events.RegisterBuilder, null);
            registered = false;
        }

        if (atBuildTarget)
        {
            //start the build
            mover.ClearDestination();
            StartBuild();
        }
    }

    public void Build(Buildable buildable, BuildAction buildAction)
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
