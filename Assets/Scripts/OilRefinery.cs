using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilRefinery : MonoBehaviour
{
    OilDeposit sourceOilDeposit;
    ResourceNode resourceNode;
    Buildable buildable;

    // Start is called before the first frame update
    void Start()
    {
        resourceNode = GetComponent<ResourceNode>();
        buildable = GetComponent<Buildable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildable && buildable.currentBuildState == BuildState.Built && !resourceNode.enabled)
        {
            ResourceNode oilDepositResourceNode = sourceOilDeposit.GetComponent<ResourceNode>();

            resourceNode.enabled = true;
            resourceNode.totalResourceAmount = oilDepositResourceNode.totalResourceAmount;
            resourceNode.availableResourceAmount = oilDepositResourceNode.availableResourceAmount;
            resourceNode.resourcesPerCollect = oilDepositResourceNode.resourcesPerCollect;
        }
    }

    public void AssignOilDeposit(OilDeposit oilDeposit)
    {
        this.sourceOilDeposit = oilDeposit;
    }
}
