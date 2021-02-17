using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Buildable : MonoBehaviour
{
    public BuildState currentBuildState = BuildState.None;
    public GameObject placeholderActor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        placeholderActor.SetActive(currentBuildState == BuildState.Placing);

        if (currentBuildState == BuildState.Placing)
        {
            NavMeshHit navMeshHit;
            Debug.Log(NavMesh.SamplePosition(transform.position, out navMeshHit, .5f, NavMesh.AllAreas));
        }
    }
}
