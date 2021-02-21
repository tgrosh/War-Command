using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : NetworkBehaviour
{
    [SyncVar]
    public int availableResourceAmount;
    public int totalResourceAmount;
    public int resourcesPerCollect = 1;

    // Start is called before the first frame update
    void Start()
    {
        availableResourceAmount = totalResourceAmount;
    }

    public int Collect()
    {
        if (availableResourceAmount > resourcesPerCollect)
        {
            availableResourceAmount -= resourcesPerCollect;
            return resourcesPerCollect;
        }
        return 0;
    }
}
