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
    }

    void ClearSelection()
    {
        foreach (Selectable selectable in selectedObjects)
        {
            selectable.isSelected = false;
        }
        selectedObjects.Clear();
    }

    RaycastHit RayCast() {
        RaycastHit hit;

        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 500);

        return hit;
    }
}
