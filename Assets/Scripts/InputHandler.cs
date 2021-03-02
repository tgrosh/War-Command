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

    GridSystem<bool> gridSystem;

    Vector2 selectionStartPosition;
    bool isDragSelecting;
    

    private void Start()
    {
        if (!isLocalPlayer) return;

        gridSystem = new GridSystem<bool>(100, 100, 5);
        EventManager.Subscribe(EventManager.EventMessage.BuildButtonPressed, BuildButtonPressed);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer) return;

        if (currentBuildable)
        {
            int sceneryMask = 1 << LayerMask.NameToLayer("Scenery");
            RaycastHit hit = RayCast(sceneryMask);

            if (hit.collider)
            {
                Vector2Int gridPosition = gridSystem.GetXY(hit.point);
                Vector3 worldPosition = gridSystem.GetWorldPosition(gridPosition.x, gridPosition.y);
                worldPosition.y = hit.point.y;
                currentBuildable.transform.position = worldPosition;                            
            }
        }

        if (!EventSystem.current.IsPointerOverGameObject(-1) && Mouse.current.leftButton.wasPressedThisFrame)
        {
            RaycastHit hit = RayCast(rayLayers);

            if (hit.collider)
            {
                if (currentBuildable)
                {
                    if (currentBuildable.currentBuildState != BuildState.InvalidPlacement) {
                        //placing buildable
                        currentBuildable.ShowPendingBuild();
                        GetCurrentBuilder().Build(currentBuildable, currentToolbarAction);
                        currentBuildable = null;
                        currentToolbarAction = null;
                    }
                }
                else
                {
                    //selecting, not placing buildable
                    Selectable selectable = hit.collider.GetComponentInParent<Selectable>();
                    if (selectable)
                    {
                        if (Keyboard.current.shiftKey.isPressed && selectable.selectionType)
                        {
                            object[] similar = GameObject.FindObjectsOfType(selectable.selectionType.GetType());

                            foreach (object o in similar)
                            {
                                Component comp = o as Component;
                                AddSelection(comp.GetComponent<Selectable>());
                            }
                        }
                        else if (Keyboard.current.ctrlKey.isPressed && selectable.selectionType)
                        {
                            ToggleSelection(selectable);
                        }
                        else
                        {
                            ClearSelection();
                            AddSelection(selectable);
                        }
                    } else
                    {
                        ClearSelection();
                        selectionStartPosition = Mouse.current.position.ReadValue();
                    }                   
                }                
            }
        }

        if (!EventSystem.current.IsPointerOverGameObject(-1) &&
            Mouse.current.leftButton.isPressed &&
            !Mouse.current.leftButton.wasPressedThisFrame)
        {
            // mouse is pressed, but wasnt pressed this frame, so dragging
            UpdateSelectionBox(selectionStartPosition, Mouse.current.position.ReadValue());
            isDragSelecting = true;
        }

        if (isDragSelecting && !Mouse.current.leftButton.isPressed)
        {
            isDragSelecting = false;
            EventManager.Emit(EventManager.EventMessage.SelectionBoxUpdated, null);
        }

        if (Mouse.current.rightButton.wasPressedThisFrame && selectedObjects.Count > 0)
        {
            RaycastHit hit = RayCast(rayLayers);

            if (hit.collider)
            {
                Targetable targetable = hit.collider.GetComponentInParent<Targetable>();

                if (targetable)
                {
                    NetworkIdentity ident = targetable.GetComponent<NetworkIdentity>();

                    if (ident && !ident.hasAuthority)
                    {
                        foreach (Mover mover in GetMovers())
                        {
                            Targeter targeter = mover.GetComponent<Targeter>();
                            if (targeter)
                            {
                                targeter.SetTarget(targetable);
                            }
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

    void UpdateSelectionBox(Vector2 startPosition, Vector2 mousePosition)
    {
        float width = mousePosition.x - startPosition.x;
        float height = mousePosition.y - startPosition.y;

        Vector2 sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        Vector2 anchoredPosition = startPosition + new Vector2(width / 2, height / 2);

        EventManager.Emit(EventManager.EventMessage.SelectionBoxUpdated, new Vector2[] {sizeDelta, anchoredPosition} );
    }

    [Command]
    private void CmdSpawnBuildable(string prefabName, Vector3 position, BuildState buildState)
    {
        GameObject prefab = FindObjectOfType<WarCommandNetworkManager>().spawnPrefabs.Find(go => go.name == prefabName);
        if (prefab)
        {
            currentBuildable = Instantiate(prefab, position, Quaternion.identity).GetComponent<Buildable>();
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
            currentBuildable.currentBuildState = BuildState.Placing;
        }
    }

    private void BuildButtonPressed(object arg0)
    {
        currentToolbarAction = arg0 as ToolbarAction;
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit))
        {
            CmdSpawnBuildable(currentToolbarAction.prefab.name, hit.point, BuildState.None);
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

    void AddSelection(Selectable selectable)
    {
        selectable.Select();
        selectedObjects.Add(selectable);
    }

    void RemoveSelection(Selectable selectable)
    {
        selectable.DeSelect();
        if (selectedObjects.Contains(selectable))
        {
            selectedObjects.Remove(selectable);
        }
    }

    void ToggleSelection(Selectable selectable)
    {
        if (selectable.IsSelected)
        {
            RemoveSelection(selectable);
        } else
        {
            AddSelection(selectable);
        }
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

    RaycastHit RayCast(LayerMask layers) {
        RaycastHit hit;

        Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 500, layers);

        return hit;
    }

    NavMeshHit GetNavMeshAtMouse(float range, int navMeshAreaMask)
    {
        RaycastHit hit = RayCast(rayLayers);
        NavMeshHit navMeshHit;

        NavMesh.SamplePosition(hit.point, out navMeshHit, range, navMeshAreaMask);

        return navMeshHit;
    }

}
