using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.VFX;

public class Collector : MonoBehaviour
{
    public VisualEffect collectorEffect;
    public float collectionRange;
    public float collectionPerSecond;
    public int currentlyCollectedResources;
    public int maxResources;

    Mover mover;
    bool atResourceTarget;
    bool isDelivering;
    ResourceNode resourceTarget;
    Targeter targeter;
    float collectionTimer;
    Base nearestBase;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<Mover>();
        targeter = GetComponent<Targeter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targeter.target)
        {
            resourceTarget = targeter.target.GetComponent<ResourceNode>();            
        }

        //if we have a resource target, and we are not full, and we are not returning to base to deliver
        if (resourceTarget && !atResourceTarget && currentlyCollectedResources < maxResources && !isDelivering)
        {
            //move to resource node and collect
            mover.SetDestination(resourceTarget.transform.position);
        }

        atResourceTarget = mover.moveComplete && resourceTarget && Vector3.Distance(transform.position, resourceTarget.transform.position) < collectionRange;

        if (atResourceTarget)
        {
            transform.LookAt(new Vector3(resourceTarget.transform.position.x, transform.position.y, resourceTarget.transform.position.z));
            if (collectionPerSecond > 0 && collectionTimer > 1/collectionPerSecond)
            {
                int collectionAmount = resourceTarget.resourcesPerCollect;
                if (currentlyCollectedResources <= maxResources - collectionAmount)
                {
                    resourceTarget.Collect();
                    currentlyCollectedResources += collectionAmount;
                } else
                {
                    //go back to base
                    Transform nearestBaseTransform = FindNearestBase();
                    if (nearestBaseTransform != null)
                    {
                        nearestBase = nearestBaseTransform.gameObject.GetComponent<Base>();
                        mover.SetDestination(nearestBase.transform.position);
                    }                    
                }
                collectionTimer = 0f;
            }
            collectionTimer += Time.deltaTime;

            collectorEffect.gameObject.SetActive(true);
        } else
        {
            collectorEffect.gameObject.SetActive(false);
        }
    }

    Transform FindNearestBase()
    {
        GameObject closest = null;
        float closestDistance = 0f;

        foreach (GameObject go in GameObject.FindGameObjectsWithTag("Base"))
        {
            Base goBase = go.GetComponent<Base>();
            if (go == null) continue;

            float goDistance = Vector3.Distance(transform.position, go.transform.position);

            if (closest == null || goDistance < closestDistance)
            {
                closest = go;
                closestDistance = goDistance;
            }
        }

        return closest.transform;
    }
}
