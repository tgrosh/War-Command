using Mirror;
using System;
using UnityEngine;
using UnityEngine.VFX;

public class Collector : NetworkBehaviour
{
    public int maxResources;
    public float collectionPerSecond;
    public float collectionRange;
    public float deliveryRange;
    public VisualEffect collectorEffect;
    public int currentlyCollectedResources;

    Mover mover;
    Targeter targeter;

    bool atResourceTarget;
    bool atDeliveryTarget;

    ResourceDepot depotTarget; // the place i will deliver to
    ResourceNode resourceTarget; // the place i will collect from

    float collectionTimer;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<Mover>();
        targeter = GetComponent<Targeter>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAuthority) return;

        if (targeter.target)
        {
            resourceTarget = targeter.target.GetComponent<ResourceNode>();
        }
        else
        {
            resourceTarget = null;
        }        

        atResourceTarget = mover.moveComplete && resourceTarget && Vector3.Distance(transform.position, resourceTarget.transform.position) < collectionRange;
        atDeliveryTarget = mover.moveComplete && depotTarget && Vector3.Distance(transform.position, depotTarget.transform.position) < deliveryRange;

        //if we have a resource target, and we are not at the resource, and we are not full
        if (resourceTarget && !atResourceTarget && CanCollectFromResource(resourceTarget))
        {
            //move to resource node
            mover.SetDestination(resourceTarget.transform.position);
        }

        //if we have a resource target, and we are at the resource, and we are not full
        if (resourceTarget && atResourceTarget && CanCollectFromResource(resourceTarget))
        {
            if (hasAuthority)
            {
                transform.LookAt(new Vector3(resourceTarget.transform.position.x, transform.position.y, resourceTarget.transform.position.z));
                if (collectionPerSecond > 0 && collectionTimer > 1 / collectionPerSecond)
                {
                    int collectionAmount = resourceTarget.resourcesPerCollect;
                    if (CanCollectFromResource(resourceTarget))
                    {
                        //collect
                        CmdCollect(resourceTarget);
                        currentlyCollectedResources += collectionAmount;
                    }
                    collectionTimer = 0f;
                }
                collectionTimer += Time.deltaTime;

            }
        }

        //if we have a resource target, and we are full, and we are not at delivery target
        if (resourceTarget && !CanCollectFromResource(resourceTarget) && !atDeliveryTarget)
        {
            //move to delivery target
            if (!depotTarget)
            {
                SetDeliveryTarget();
            }

            if (depotTarget)
            {
                mover.SetDestination(depotTarget.transform.position);
            }
        }

        //if we have a delivery target, and we are at the delivery target, and we have resources to deliver
        if (depotTarget && atDeliveryTarget && currentlyCollectedResources > 0)
        {
            //deliver
            Deliver();
        }

        collectorEffect.gameObject.SetActive(resourceTarget && atResourceTarget && CanCollectFromResource(resourceTarget));
    }

    bool CanCollectFromResource(ResourceNode resource)
    {
        return currentlyCollectedResources <= maxResources - resource.resourcesPerCollect;
    }

    [Command]
    void CmdCollect(ResourceNode node)
    {
        node.Collect();
    }

    void Deliver()
    {
        if (resourceTarget.GetType() == typeof(IronOre))
        {
            depotTarget.DepositIron(currentlyCollectedResources);
        } else if (resourceTarget.GetComponent<OilRefinery>())
        {
            depotTarget.DepositOil(currentlyCollectedResources);
        }
        currentlyCollectedResources = 0;
    }

    void SetDeliveryTarget()
    {
        Transform nearest = FindNearestResourceDepot();
        if (nearest)
        {
            depotTarget = FindNearestResourceDepot().GetComponent<ResourceDepot>();
        }
    }

    Transform FindNearestResourceDepot()
    {
        ResourceDepot closestDepot = null;
        float closestDistance = 0f;

        foreach (ResourceDepot depot in GameObject.FindObjectsOfType<ResourceDepot>())
        {
            NetworkIdentity ident = depot.GetComponent<NetworkIdentity>();
            if (!ident || !ident.hasAuthority) continue; //only consider friendly depots

            float depotDistance = Vector3.Distance(transform.position, depot.transform.position);

            if (closestDepot == null || depotDistance < closestDistance)
            {
                closestDepot = depot;
                closestDistance = depotDistance;
            }
        }

        return closestDepot ? closestDepot.transform : null;
    }

}
