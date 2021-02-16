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
                    foreach (Mover mover in GetMovers())
                    {
                        mover.ClearDestination();
                        mover.SetDestinationPosition(hit.collider.transform.position);

                        Targeter targeter = mover.GetComponent<Targeter>();
                        if (targeter)
                        {
                            targeter.SetTarget(hit.collider.transform);
                        }
                    }
                }
                else
                {
                    foreach (Mover mover in GetMovers())
                    {
                        mover.SetDestinationPosition(hit.point);
                        mover.ShowDestinationMarker(hit.point);

                        Targeter targeter = mover.GetComponent<Targeter>();
                        if (targeter)
                        {
                            targeter.ClearTarget();
                        }
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

    List<Mover> GetMovers()
    {
        List<Mover> movers = new List<Mover>();

        foreach (Selectable selectable in selectedObjects)
        {
            Mover moveable = selectable.GetComponent<Mover>();
            if (moveable)
            {
                movers.Add(moveable);
            }
        }

        return movers;
    }

    RaycastHit RayCast() {
        RaycastHit hit;

        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 500);

        return hit;
    }
}
