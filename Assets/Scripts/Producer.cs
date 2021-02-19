using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Producer : MonoBehaviour
{
    public float producerRange;

    private void Start()
    {
        EventManager.Subscribe(EventManager.EventMessage.ProducerButtonPressed, ProducerButtonPressed);
    }

    private void ProducerButtonPressed(object arg0)
    {
        ToolbarAction action = arg0 as ToolbarAction;
        NavMeshHit navHit;
        NavMesh.SamplePosition(transform.position, out navHit, producerRange, NavMesh.AllAreas);

        if (navHit.hit)
        {
            if (ResourceBank.Withdraw(action.cost))
            {
                Instantiate(action.prefab, navHit.position, Quaternion.identity);
            }
        }
    }

    public Vector3 RandomNavmeshLocation(float radius)
    {
        Vector3 randomDirection = Random.insideUnitSphere * radius;
        randomDirection += transform.position;
        NavMeshHit hit;
        Vector3 finalPosition = transform.position;
        if (NavMesh.SamplePosition(randomDirection, out hit, radius, NavMesh.AllAreas))
        {
            finalPosition = hit.position;
        }
        return finalPosition;
    }
}
