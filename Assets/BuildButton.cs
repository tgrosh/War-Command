using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildButton : MonoBehaviour
{
    public string buildId;

    public void ToolbarButtonPressed()
    {
        EventManager.Emit(EventManager.Events.ToolbarButtonPressed, buildId); 
    }
}
