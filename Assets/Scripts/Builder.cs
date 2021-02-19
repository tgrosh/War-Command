using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public List<BuildAction> buildMenuItems = new List<BuildAction>();

    Mover mover;
    Selectable selectable;
    Buildable currentBuildable;

    bool registered;

    private void Start()
    {
        mover = GetComponent<Mover>();
        selectable = GetComponent<Selectable>();
    }

    private void Update()
    {
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

        if (currentBuildable && mover.moveComplete)
        {
            //start the build
            mover.ClearDestination();
            StartBuild();
        }
    }

    public void Build(Buildable buildable)
    {
        currentBuildable = buildable;

        // goto buildable location
        mover.SetDestination(buildable.transform.position);
    }

    void StartBuild()
    {
        currentBuildable.Build();
        currentBuildable = null;
    }

}
