using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public int availableResourceAmount;
    public int totalResourceAmount;

    // Start is called before the first frame update
    void Start()
    {
        availableResourceAmount = totalResourceAmount;
    }

    public int Collect()
    {
        availableResourceAmount--;
        return availableResourceAmount > 0 ? 1: 0;
    }
}
