using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class ClickToMove : MonoBehaviour
{
    public GameObject locationMarkerPrefab;

    NavMeshAgent agent;
    GameObject locationMarker;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue()), out hit, 100))
            {
                if (hit.collider.GetComponent<MoveTarget>())
                {
                    SetDestination(hit.transform.position);
                }
                SetDestination(hit.point);
            }
        }

        if (agent.hasPath && agent.remainingDistance < agent.stoppingDistance)
        {
            Destroy(locationMarker);
        }
    }

    void SetDestination(Vector3 targetPosition)
    {
        agent.destination = targetPosition;
        if (locationMarker)
        {
            Destroy(locationMarker);
        }
        locationMarker = Instantiate(locationMarkerPrefab, targetPosition+(Vector3.up*1.55f), Quaternion.Euler(Vector3.right*90));
    }
        
}
