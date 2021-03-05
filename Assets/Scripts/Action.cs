using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public ActionType actionType;
    public GameObject actionTarget;
    public Vector3 actionPosition;

    public Action(ActionType actionType, GameObject actionTarget)
    {
        this.actionType = actionType;
        this.actionTarget = actionTarget;
    }

    public Action(ActionType actionType, Vector3 actionPosition)
    {
        this.actionType = actionType;
        this.actionPosition = actionPosition;
    }
}
