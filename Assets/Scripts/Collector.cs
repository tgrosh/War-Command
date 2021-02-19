using UnityEngine;
using UnityEngine.VFX;

public class Collector : MonoBehaviour
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

    Base deliveryTarget; // the place i will deliver to
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
        if (targeter.target)
        {
            resourceTarget = targeter.target.GetComponent<ResourceNode>();
        }

        atResourceTarget = mover.moveComplete && resourceTarget && Vector3.Distance(transform.position, resourceTarget.transform.position) < collectionRange;
        atDeliveryTarget = mover.moveComplete && deliveryTarget && Vector3.Distance(transform.position, deliveryTarget.transform.position) < deliveryRange;

        //if we have a resource target, and we are not at the resource, and we are not full
        if (resourceTarget && !atResourceTarget && CanCollectFromResource(resourceTarget))
        {
            //move to resource node
            mover.SetDestination(resourceTarget.transform.position);
        }

        //if we have a resource target, and we are at the resource, and we are not full
        if (resourceTarget && atResourceTarget && CanCollectFromResource(resourceTarget))
        {
            //collect
            Collect();
        }

        //if we have a resource target, and we are full, and we are not at delivery target
        if (resourceTarget && !CanCollectFromResource(resourceTarget) && !atDeliveryTarget)
        {
            //move to delivery target
            if (!deliveryTarget)
            {
                deliveryTarget = FindNearestBase().GetComponent<Base>();
            }
            mover.SetDestination(deliveryTarget.transform.position);
        }

        //if we dont have a resource target, and we have any resources, and we are not at delivery target
        if (!resourceTarget && currentlyCollectedResources > 0 && !atDeliveryTarget)
        {
            //move to delivery target
            if (!deliveryTarget)
            {
                deliveryTarget = FindNearestBase().GetComponent<Base>();
            }
            mover.SetDestination(deliveryTarget.transform.position);
        }

        //if we have a delivery target, and we are at the delivery target, and we have resources to deliver
        if (deliveryTarget && atDeliveryTarget && currentlyCollectedResources > 0)
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

    void Collect()
    {
        transform.LookAt(new Vector3(resourceTarget.transform.position.x, transform.position.y, resourceTarget.transform.position.z));
        if (collectionPerSecond > 0 && collectionTimer > 1 / collectionPerSecond)
        {
            int collectionAmount = resourceTarget.resourcesPerCollect;
            if (CanCollectFromResource(resourceTarget))
            {
                resourceTarget.Collect();
                currentlyCollectedResources += collectionAmount;
            }
            collectionTimer = 0f;
        }
        collectionTimer += Time.deltaTime;
    }

    void Deliver()
    {
        EventManager.Emit(EventManager.Events.ResourceCollected, currentlyCollectedResources);
        currentlyCollectedResources = 0;
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
