using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputHandler : NetworkBehaviour
{
    public List<Selectable> selectedObjects = new List<Selectable>();
    public ToolbarAction currentToolbarAction;
    public Buildable currentBuildable;
    public LayerMask rayLayers;    

    private void Start()
    {
        if (!isLocalPlayer) return;

        EventManager.Subscribe(EventManager.EventMessage.BuildButtonPressed, BuildButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

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
                    GetCurrentBuilder().Build(currentBuildable, currentToolbarAction);
                    currentBuildable = null;
                    currentToolbarAction = null;
                }
                else
                {
                    //not placing buildable
                    ClearSelection();
                    Selectable selectable = hit.collider.GetComponentInParent<Selectable>();

                    if (selectable)
                    {
                        selectable.Select();
                        selectedObjects.Add(selectable);
                    }
                }                
            }
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && selectedObjects.Count > 0)
        {
            RaycastHit hit = RayCast();

            if (hit.collider)
            {
                if (hit.collider.GetComponentInParent<Targetable>())
                {
                    foreach (Mover mover in GetMovers())
                    {
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

    [Command]
    private void CmdSpawnBuildable(string prefabName, Vector3 position, BuildState buildState)
    {
        GameObject prefab = FindObjectOfType<WarCommandNetworkManager>().spawnPrefabs.Find(go => go.name == prefabName);
        if (prefab)
        {
            currentBuildable = Instantiate(prefab, position, transform.rotation).GetComponent<Buildable>();
            currentBuildable.currentBuildState = buildState;
        }
        NetworkServer.Spawn(currentBuildable.gameObject, connectionToClient);
        RpcSetCurrentBuildable(currentBuildable);
    }

    [ClientRpc]
    void RpcSetCurrentBuildable(Buildable buildable)
    {
        if (isLocalPlayer)
        {
            currentBuildable = buildable;
        }
    }

    private void BuildButtonPressed(object arg0)
    {
        currentToolbarAction = arg0 as ToolbarAction;
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit))
        {
            CmdSpawnBuildable(currentToolbarAction.prefab.name, hit.point, BuildState.Placing);
        }
    }

    void ClearSelection()
    {
        foreach (Selectable selectable in selectedObjects)
        {
            selectable.DeSelect();
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
