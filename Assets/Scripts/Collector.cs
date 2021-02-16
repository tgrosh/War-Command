using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Collector : MonoBehaviour
{
    public VisualEffect collectorEffect;
    public float collectionPerSecond;
    public float collectionFacingSpeed;

    NavMeshAgent agent;
    bool atResourceTarget;
    ResourceNode resourceTarget;
    Targeter targeter;
    float collectionTimer;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        targeter = GetComponent<Targeter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targeter.target)
        {
            resourceTarget = targeter.target.GetComponent<ResourceNode>();
        } else
        {
            resourceTarget = null;
        }

        if (!resourceTarget)
        {
            atResourceTarget = false;
        }

        if (atResourceTarget)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(resourceTarget.transform.position - transform.position), Time.deltaTime * collectionFacingSpeed);
            if (collectionPerSecond > 0 && collectionTimer > 1/collectionPerSecond)
            {
                EventManager.Emit(EventManager.Events.ResourceCollected, 1);
                collectionTimer = 0f;
            }
            collectionTimer += Time.deltaTime;

            collectorEffect.gameObject.SetActive(true);
        } else
        {
            collectorEffect.gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (resourceTarget && other.gameObject == resourceTarget.gameObject && agent.remainingDistance <= agent.stoppingDistance)
        {
            atResourceTarget = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (resourceTarget && other.gameObject == resourceTarget.gameObject)
        {
            atResourceTarget = false;
        }
    }
}
