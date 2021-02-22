using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolbarProvider : MonoBehaviour
{
    public List<ToolbarAction> toolbarActions = new List<ToolbarAction>();

    Selectable selectable;
    bool registered;

    private void Start()
    {
        selectable = GetComponent<Selectable>();
    }

    void Update()
    {
        if (selectable && selectable.IsSelected && !registered)
        {
            EventManager.Emit(EventManager.EventMessage.RegisterToolbarProvider, this);
            registered = true;
        }

        if (!selectable || (!selectable.IsSelected && registered))
        {
            EventManager.Emit(EventManager.EventMessage.UnRegisterToolbarProvider, this);
            registered = false;
        }
    }
}
