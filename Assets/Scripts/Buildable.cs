using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.SceneManagement;
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

    [SyncVar]
    Vector3 buildPosition = Vector3.zero;

    Vector3 previousPosition;
    Vector3 extents;

    // Start is called before the first frame update
    void Start()
    {
        extents = actor.GetComponent<NavMeshObstacle>().size;
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
        CmdSetBuildState(BuildState.PendingBuild, transform.position);
    }

    [Command]
    public void CmdSetBuildState(BuildState buildState, Vector3 position)
    {
        buildPosition = position;
        currentBuildState = buildState;
    }

    public void Build()
    {
        CmdSetBuildState(BuildState.Built, transform.position); //do more later
    }

    public void SetActor(GameObject actor, bool isActive)
    {
        actor.SetActive(isActive);
        if (isActive) currentActor = actor;
    }

    public bool CanBuildHere() {
        Collider[] colliders = Physics.OverlapBox(transform.position, extents, transform.rotation, invalidCollisions);
        Debug.Log(colliders.Length);
        return GetNavMeshAtCurrentPosition().hit && colliders.Length == 0;
    }

    NavMeshHit GetNavMeshAtCurrentPosition()
    {
        int placeableMask = 1 << NavMesh.GetAreaFromName("Placeable");
        NavMeshHit navMeshHit;

        NavMesh.SamplePosition(transform.position, out navMeshHit, .25f, placeableMask);

        return navMeshHit;
    }

}
