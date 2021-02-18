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
                int placeableMask = 1 << NavMesh.GetAreaFromName("Placeable");
                NavMeshHit navMeshHit = GetNavMeshAtMouse(.25f, placeableMask);

                if (navMeshHit.hit)
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
                if (currentBuildable)
                {
                    //placing buildable
                    currentBuildable.ShowPendingBuild();
                    GetCurrentBuilder().Build(currentBuildable);
                    currentBuildable = null;
                }
                else
                {
                    //not placing buildable
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
                        mover.SetDestination(hit.collider.transform.position, hit.collider.bounds.size.magnitude * 2f);                        

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
                        mover.SetDestination(hit.point);
                        mover.ShowDestinationMarker();

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

    Builder GetCurrentBuilder()
    {
        return selectedObjects.Find(selectable => selectable.GetComponent<Builder>() != null ).GetComponent<Builder>();
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

    NavMeshHit GetNavMeshAtMouse(float range, int navMeshAreaMask)
    {
        RaycastHit hit = RayCast();
        NavMeshHit navMeshHit;

        NavMesh.SamplePosition(hit.point, out navMeshHit, range, navMeshAreaMask);

        return navMeshHit;
    }

}
