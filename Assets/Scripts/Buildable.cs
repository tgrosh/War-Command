using Mirror;
using System.Collections;
using System.Collections.Generic;
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

    [SyncVar]
    Vector3 buildPosition;
    

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
}
