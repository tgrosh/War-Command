using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public List<Selectable> selectedObjects = new List<Selectable>();

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit hit = RayCast();

            if (hit.collider)
            {
                ClearSelection();
                Selectable selectable = hit.collider.GetComponent<Selectable>();

                if (selectable)
                {
                    selectable.isSelected = true;
                    selectedObjects.Add(selectable);
                }
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && selectedObjects.Count > 0)
        {
            RaycastHit hit = RayCast();

            if (hit.collider)
            {
                if (hit.collider.GetComponent<MoveTarget>())
                {
                    foreach (Moveable moveable in GetMoveables())
                    {
                        moveable.SetDestinationTarget(hit.collider.transform);
                    }
                }
                else
                {
                    foreach (Moveable moveable in GetMoveables())
                    {
                        moveable.SetDestinationPosition(hit.point);
                    }
                }
            }
        }
    }

    void ClearSelection()
    {
        foreach (Selectable selectable in selectedObjects)
        {
            selectable.isSelected = false;
        }
        selectedObjects.Clear();
    }

    List<Moveable> GetMoveables()
    {
        List<Moveable> moveables = new List<Moveable>();

        foreach (Selectable selectable in selectedObjects)
        {
            Moveable moveable = selectable.GetComponent<Moveable>();
            if (moveable)
            {
                moveables.Add(moveable);
            }
        }

        return moveables;
    }

    RaycastHit RayCast() {
        RaycastHit hit;

        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 500);

        return hit;
    }
}
