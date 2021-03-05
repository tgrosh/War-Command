using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action
{
    public ActionType actionType;
    public GameObject actionTarget;

    public Action(ActionType actionType, GameObject actionTarget)
    {
        this.actionType = actionType;
        this.actionTarget = actionTarget;
    }
}
