using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepot : MonoBehaviour
{
    public float deliveryRange;

    public void DepositIron(int amount)
    {
        ResourceBank.DepositIron(amount);
    }

    public void DepositOil(int amount)
    {
        ResourceBank.DepositOil(amount);
    }
}
