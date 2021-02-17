using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public List<BuildAction> buildMenuItems = new List<BuildAction>();

    private void Start()
    {
    }

    private void Update()
    {
        
    }

    public void Select()
    {
        EventManager.Emit(EventManager.Events.RegisterBuilder, this);
    }

    public void Unselect()
    {
        EventManager.Emit(EventManager.Events.RegisterBuilder, null);
    }
}
