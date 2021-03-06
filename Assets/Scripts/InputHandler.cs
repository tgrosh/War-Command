using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
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
    public float dragThreshold;

    GridSystem<bool> gridSystem;

    Vector2 selectionStartPosition;
    bool isDragSelecting;
    Vector2 selectionSizeDelta;
    Vector2 selectionAnchoredPosition;
    OilDeposit closestNearbyOilDeposit;

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

        // buildable following cursor
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

                if (currentBuildable.buildOn == BuildOn.OilDeposit)
                {
                    closestNearbyOilDeposit = GetClosestNearbyOilDeposit(currentBuildable.transform.position);
                    if (closestNearbyOilDeposit)
                    {
                        currentBuildable.transform.position = closestNearbyOilDeposit.transform.position;
                        currentBuildable.transform.rotation = closestNearbyOilDeposit.transform.rotation;
                    }
                }
            }            
        }

        // left mouse click
        if (Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject(-1))
        {
            selectionStartPosition = Mouse.current.position.ReadValue();

            RaycastHit hit = RayCast(rayLayers);

            if (hit.collider)
            {
                if (currentBuildable)
                {
                    if (currentBuildable.currentBuildState != BuildState.InvalidPlacement) {
                        OilRefinery refinery = currentBuildable.GetComponent<OilRefinery>();
                        if (refinery)
                        {
                            refinery.AssignOilDeposit(closestNearbyOilDeposit);
                        }

                        //placing buildable
                        currentBuildable.ShowPendingBuild();
                        GetCurrentBuilder().Build(currentBuildable);
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
                        if (Keyboard.current.ctrlKey.isPressed && selectable.selectionType)
                        {
                            //select all similar units
                            object[] similar = GameObject.FindObjectsOfType(selectable.selectionType.GetType());

                            foreach (object o in similar)
                            {
                                Component comp = o as Component;
                                AddSelection(comp.GetComponent<Selectable>());
                            }
                        }
                        else if (Keyboard.current.shiftKey.isPressed && selectable.selectionType)
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
                    }                   
                }                
            }
        }

        // right mouse click
        if (Mouse.current.rightButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject(-1))
        {
            if (currentBuildable)
            {
                currentBuildable.CancelBuild();
                currentBuildable = null;
            } else
            {
                RaycastHit hit = RayCast(rayLayers);

                if (selectedObjects.Count > 0)
                {
                    if (hit.collider)
                    {
                        Action action;

                        //if scenery... create move action
                        if (hit.collider.transform.root.gameObject.layer == LayerMask.NameToLayer("Scenery"))
                        {
                            action = new Action(ActionType.Move, hit.point);
                        } else
                        {
                            action = GetAction(hit.collider.gameObject);
                        }

                        if (action != null)
                        {
                            foreach (ActionQueue queue in GetSelectedActionQueues())
                            {
                                queue.Clear();
                                queue.Add(action);
                            }
                        }
                    }
                } else
                {
                    if (hit.collider)
                    {
                        Buildable buildable = hit.collider.GetComponentInParent<Buildable>();
                        if (buildable && buildable.currentBuildState == BuildState.PendingBuild)
                        {
                            buildable.CancelBuild();
                        }
                    }
                }
            }
        }

        // holding left mouse
        if (Mouse.current.leftButton.isPressed && !Mouse.current.leftButton.wasPressedThisFrame && !EventSystem.current.IsPointerOverGameObject(-1))
        {
            if (Vector2.Distance(selectionStartPosition, Mouse.current.position.ReadValue()) > dragThreshold)
            {
                // mouse is pressed, but wasnt pressed this frame, so dragging
                UpdateSelectionBox(selectionStartPosition, Mouse.current.position.ReadValue());
                isDragSelecting = true;

                SelectSelectablesInBox();
            }
        }

        // releasing left mouse while drag selecting
        if (isDragSelecting && !Mouse.current.leftButton.isPressed)
        {
            isDragSelecting = false;
            EventManager.Emit(EventManager.EventMessage.SelectionBoxUpdated, null);
        }
    }

    Action GetAction(GameObject targetGameObject)
    {
        NetworkIdentity identity = targetGameObject.GetComponentInParent<NetworkIdentity>();

        //if attackable, and not friendly... create attack action
        Attackable attackable = targetGameObject.GetComponentInParent<Attackable>();
        if (attackable && identity && !identity.hasAuthority)
        {
            return new Action(ActionType.Attack, attackable.gameObject);
        }

        //if buildable, and friendly... create build action
        Buildable buildable = targetGameObject.GetComponentInParent<Buildable>();
        if (buildable && buildable.currentBuildState == BuildState.PendingBuild && identity && identity.hasAuthority)
        {
            return new Action(ActionType.Build, buildable.gameObject);
        }

        //if resource node, create collect action
        ResourceNode resourceNode = targetGameObject.GetComponentInParent<ResourceNode>();
        if (resourceNode)
        {
            return new Action(ActionType.Collect, resourceNode.gameObject);
        }

        //if oil refinery, and friendly... create collect action
        OilRefinery refinery = targetGameObject.GetComponentInParent<OilRefinery>();
        if (refinery && identity && identity.hasAuthority)
        {
            return new Action(ActionType.Collect, refinery.gameObject);
        }

        return null;
    }

    private OilDeposit GetClosestNearbyOilDeposit(Vector3 currentPosition)
    {
        OilDeposit closestNearbyOilDeposit = null;

        OilDeposit[] oilDeposits = FindObjectsOfType<OilDeposit>();
        foreach (OilDeposit oilDeposit in oilDeposits)
        {
            float distanceToOilDeposit = Vector3.Distance(currentPosition, oilDeposit.transform.position);
            if (distanceToOilDeposit < 12f && (closestNearbyOilDeposit == null || distanceToOilDeposit < Vector3.Distance(currentPosition, closestNearbyOilDeposit.transform.position)))
            {
                closestNearbyOilDeposit = oilDeposit;
            }
        }

        return closestNearbyOilDeposit;
    }

    void UpdateSelectionBox(Vector2 startPosition, Vector2 mousePosition)
    {
        float width = mousePosition.x - startPosition.x;
        float height = mousePosition.y - startPosition.y;

        selectionSizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        selectionAnchoredPosition = startPosition + new Vector2(width / 2, height / 2);

        EventManager.Emit(EventManager.EventMessage.SelectionBoxUpdated, new Vector2[] {selectionSizeDelta, selectionAnchoredPosition} );
    }

    void SelectSelectablesInBox()
    {
        ClearSelection();

        Vector2 min = selectionAnchoredPosition - (selectionSizeDelta / 2);
        Vector2 max = selectionAnchoredPosition + (selectionSizeDelta / 2);

        foreach (Selectable selectable in FindObjectsOfType<Selectable>())
        {
            Vector3 screenPosition = Camera.main.WorldToScreenPoint(selectable.transform.position);

            if (screenPosition.x > min.x && screenPosition.x < max.x && screenPosition.y > min.y && screenPosition.y < max.y)
            {
                AddSelection(selectable);
            }
        }
    }

    [Command]
    private void CmdSpawnBuildable(string prefabName, Vector3 position, int ironCost, int oilCost)
    {
        GameObject prefab = FindObjectOfType<WarCommandNetworkManager>().spawnPrefabs.Find(go => go.name == prefabName);
        if (prefab)
        {
            currentBuildable = Instantiate(prefab, position, Quaternion.identity).GetComponent<Buildable>();
            currentBuildable.currentBuildState = BuildState.None;
            currentBuildable.ironCost = ironCost;
            currentBuildable.oilCost = oilCost;
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
            CmdSpawnBuildable(currentToolbarAction.prefab.name, hit.point, currentToolbarAction.ironCost, currentToolbarAction.oilCost);
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
        if (selectable.Select())
        {
            selectedObjects.Add(selectable);
        }
    }

    void RemoveSelection(Selectable selectable)
    {
        if (selectable.DeSelect())
        {
            if (selectedObjects.Contains(selectable))
            {
                selectedObjects.Remove(selectable);
            }
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

    List<ActionQueue> GetSelectedActionQueues()
    {
        List<ActionQueue> queues = new List<ActionQueue>();

        foreach (Selectable selectable in selectedObjects)
        {
            ActionQueue queue = selectable.GetComponent<ActionQueue>();
            if (queue)
            {
                queues.Add(queue);
            }
        }

        return queues;
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
