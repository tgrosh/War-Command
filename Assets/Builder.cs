using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public List<BuildMenuItem> buildMenuItems = new List<BuildMenuItem>();    

    public void Select()
    {
        EventManager.Emit(EventManager.Events.RegisterBuildMenuItems, buildMenuItems);
    }
}
