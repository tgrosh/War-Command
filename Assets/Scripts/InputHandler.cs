using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    public List<Selectable> selectedObjects = new List<Selectable>();
    public Buildable currentBuildable;
    public LayerMask rayLayers;    

    private void Start()
    {
        EventManager.Subscribe(EventManager.Events.BuildButtonPressed, BuildButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentBuildable)
        {
            RaycastHit hit = RayCast();

            if (hit.collider)
            {
                NavMeshHit navMeshHit;
                int placeableMask = 1 << NavMesh.GetAreaFromName("Placeable");
                if (NavMesh.SamplePosition(hit.point, out navMeshHit, .25f, placeableMask))
                {
                    currentBuildable.transform.position = navMeshHit.position;
                    currentBuildable.currentBuildState = BuildState.Placing;
                } else
                {
                    currentBuildable.transform.position = hit.point;
                    currentBuildable.currentBuildState = BuildState.InvalidPlacement;
                }
            }
        }

        if (!EventSystem.current.IsPointerOverGameObject(-1) && Mouse.current.leftButton.wasPressedThisFrame)
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

                Builder builder = hit.collider.GetComponent<Builder>();
                if (builder)
                {
                    builder.Select();
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

    private void BuildButtonPressed(object arg0)
    {
        BuildAction buildAction = arg0 as BuildAction;
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit))
        {
            currentBuildable = Instantiate(buildAction.buildable, hit.point, transform.rotation).GetComponent<Buildable>();
            currentBuildable.currentBuildState = BuildState.Placing;
        }
    }

    void ClearSelection()
    {
        foreach (Selectable selectable in selectedObjects)
        {
            selectable.isSelected = false;

            Builder builder = selectable.GetComponent<Builder>();
            if (builder)
            {
                builder.Unselect();
            }
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

        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 500, rayLayers);

        return hit;
    }
}
