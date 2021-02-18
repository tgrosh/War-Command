using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Buildable : MonoBehaviour
{
    public BuildState currentBuildState = BuildState.None;
    public GameObject placeholderActor;
    public GameObject placeholderInvalidActor;
    public GameObject inProgressActor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        placeholderActor.SetActive(currentBuildState == BuildState.Placing);
        placeholderInvalidActor.SetActive(currentBuildState == BuildState.InvalidPlacement);
        inProgressActor.SetActive(currentBuildState == BuildState.PendingBuild);
    }

    public void ShowPendingBuild()
    {
        currentBuildState = BuildState.PendingBuild;
    }
}
