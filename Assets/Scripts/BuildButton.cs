using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public BuildAction buildAction;

    public void ToolbarButtonPressed()
    {
        EventManager.Emit(EventManager.Events.BuildButtonPressed, buildAction); 
    }
}
