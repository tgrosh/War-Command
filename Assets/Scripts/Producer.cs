using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class Producer : NetworkBehaviour
{
    public float producerRange;
    Selectable selectable;

    private void Start()
    {
        selectable = GetComponent<Selectable>();
        EventManager.Subscribe(EventManager.EventMessage.ProducerButtonPressed, ProducerButtonPressed);
    }

    private void ProducerButtonPressed(object arg0)
    {
        if (!selectable.IsSelected) return;

        ToolbarAction action = arg0 as ToolbarAction;
        NavMeshHit navHit;
        NavMesh.SamplePosition(transform.position, out navHit, producerRange, NavMesh.AllAreas);

        if (navHit.hit)
        {
            if (ResourceBank.Withdraw(action.ironCost, action.oilCost))
            {
                CmdSpawnProducable(action.prefab.name, navHit.position, Quaternion.LookRotation(navHit.position - transform.position));                
            }
        }
    }

    [Command]
    private void CmdSpawnProducable(string prefabName, Vector3 position, Quaternion rotation)
    {
        GameObject prefab = FindObjectOfType<WarCommandNetworkManager>().spawnPrefabs.Find(go => go.name == prefabName);
        if (prefab)
        {
            GameObject obj = Instantiate(prefab, position, rotation);
            NetworkServer.Spawn(obj, connectionToClient);
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
