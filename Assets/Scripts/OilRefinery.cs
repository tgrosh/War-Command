using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OilRefinery : ResourceNode
{
    OilDeposit sourceOilDeposit;
    Buildable buildable;

    // Start is called before the first frame update
    void Start()
    {
        buildable = GetComponent<Buildable>();
    }

    // Update is called once per frame
    void Update()
    {
        if (buildable && buildable.currentBuildState == BuildState.Built && sourceOilDeposit.enabled)
        {
            totalResourceAmount = sourceOilDeposit.totalResourceAmount;
            availableResourceAmount = sourceOilDeposit.availableResourceAmount;
            resourcesPerCollect = sourceOilDeposit.resourcesPerCollect;

            sourceOilDeposit.enabled = false;
        }
    }

    private void OnDestroy()
    {
        sourceOilDeposit.enabled = true;
        sourceOilDeposit.availableResourceAmount = availableResourceAmount;
    }

    public void AssignOilDeposit(OilDeposit oilDeposit)
    {
        this.sourceOilDeposit = oilDeposit;
    }
}
