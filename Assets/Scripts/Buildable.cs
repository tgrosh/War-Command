using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Buildable : NetworkBehaviour
{
    [SyncVar]
    public BuildState currentBuildState = BuildState.None;
    public GameObject actor;
    public GameObject placeholderActor;
    public GameObject placeholderInvalidActor;
    public GameObject inProgressActor;
    public GameObject currentActor;
    public LayerMask invalidCollisions;
    public BuildOn buildOn = BuildOn.Scenery;
    public int ironCost;
    public int oilCost;

    [SyncVar]
    Vector3 buildPosition = Vector3.zero;

    Vector3 previousPosition;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        SetActor(placeholderActor, (currentBuildState == BuildState.Placing));
        SetActor(placeholderInvalidActor, (currentBuildState == BuildState.InvalidPlacement));
        SetActor(inProgressActor, (currentBuildState == BuildState.PendingBuild));
        SetActor(actor, (currentBuildState == BuildState.Built));

        if (currentBuildState == BuildState.PendingBuild || currentBuildState == BuildState.Built)
        {
            transform.position = buildPosition;
        }

        if (transform.position != previousPosition && (currentBuildState == BuildState.Placing || currentBuildState == BuildState.InvalidPlacement)) {
            currentBuildState = CanBuildHere() == true ? BuildState.Placing : BuildState.InvalidPlacement;
            previousPosition = transform.position;
        }
    }

    public void ShowPendingBuild()
    {
        if (ResourceBank.Withdraw(ironCost, oilCost))
        {
            CmdSetBuildState(BuildState.PendingBuild, transform.position);
        }
    }

    [Command]
    public void CmdSetBuildState(BuildState buildState, Vector3 position)
    {
        buildPosition = position;
        currentBuildState = buildState;
    }

    public void Build()
    {
        buildPosition = transform.position;
        currentBuildState = BuildState.Built;

        if (hasAuthority)
        {
            CmdSetBuildState(BuildState.Built, transform.position); //do more later
        }
    }

    public void CancelBuild()
    {
        ResourceBank.DepositIron(ironCost);
        ResourceBank.DepositOil(oilCost);
        Destroy(gameObject);
    }

    public void SetActor(GameObject actor, bool isActive)
    {
        actor.SetActive(isActive);
        if (isActive) currentActor = actor;
    }

    public bool CanBuildHere() {
        if (buildOn == BuildOn.Scenery)
        {
            Collider[] colliders = Physics.OverlapBox(transform.position, GetBounds().extents * .75f, transform.rotation, invalidCollisions);
            return GetNavMeshAtCurrentPosition().hit && colliders.Length == 0;
        } else
        {
            OilDeposit[] oilDeposits = FindObjectsOfType<OilDeposit>();
            foreach (OilDeposit oilDeposit in oilDeposits)
            {
                if (oilDeposit.transform.position == transform.position)
                {
                    return true;
                }
            }
            return false;
        }
    }

    NavMeshHit GetNavMeshAtCurrentPosition()
    {
        int placeableMask = 1 << NavMesh.GetAreaFromName("Placeable");
        NavMeshHit navMeshHit;

        NavMesh.SamplePosition(transform.position, out navMeshHit, .25f, placeableMask);

        return navMeshHit;
    }

    public Bounds GetBounds()
    {
        Bounds combinedBounds = new Bounds(transform.position, Vector3.zero);
        foreach (Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            combinedBounds.Encapsulate(renderer.bounds);
        }

        return combinedBounds;
    }

}
