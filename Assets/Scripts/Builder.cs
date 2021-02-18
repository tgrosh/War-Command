using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Builder : MonoBehaviour
{
    public List<BuildAction> buildMenuItems = new List<BuildAction>();

    Mover mover;
    Targeter targeter;
    Buildable currentBuildable;

    private void Start()
    {
        mover = GetComponent<Mover>();
        targeter = GetComponent<Targeter>();
    }

    private void Update()
    {
        if (currentBuildable && mover.moveComplete)
        {
            //start the build
            mover.ClearDestination();
            StartBuild();
        }
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
