using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public List<BuildMenuItem> buildMenuItems = new List<BuildMenuItem>();

    private void Start()
    {
        EventManager.Subscribe(EventManager.Events.ToolbarButtonPressed, ToolbarButtonPressed);
    }

    private void ToolbarButtonPressed(object arg0)
    {
        string id = arg0 as string;
        
        // do something with that id
    }

    public void Select()
    {
        EventManager.Emit(EventManager.Events.RegisterBuildMenuItems, buildMenuItems);
    }
}
