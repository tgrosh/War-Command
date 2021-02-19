using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepot : MonoBehaviour
{
    int currentResources;

    public void Deposit(int amount)
    {
        currentResources += amount;
        EventManager.Emit(EventManager.Events.ResourceAmountChanged, currentResources);
    }

    public bool Withdraw(int amount)
    {
        if (currentResources < amount) return false;

        currentResources -= amount;
        EventManager.Emit(EventManager.Events.ResourceAmountChanged, currentResources);
        return true;
    }
}
