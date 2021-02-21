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
    }

    public void ShowPendingBuild()
    {
        currentBuildState = BuildState.PendingBuild;
    }

    public void Build()
    {
        currentBuildState = BuildState.Built; //do more later
    }

    public void SetActor(GameObject actor, bool isActive)
    {
        actor.SetActive(isActive);
        if (isActive) currentActor = actor;
    }
}
