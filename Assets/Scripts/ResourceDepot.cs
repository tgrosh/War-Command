using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepot : MonoBehaviour
{
    public void Deposit(int amount)
    {
        ResourceBank.Deposit(amount);
    }
}
