using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceDepot : MonoBehaviour
{
    public static int currentResources;

    public void Deposit(int amount)
    {
        currentResources += amount;
        EventManager.Emit(EventManager.Events.ResourcesDeposited, amount);
    }

    public bool Withdraw(int amount)
    {
        if (currentResources < amount) return false;

        currentResources -= amount;
        EventManager.Emit(EventManager.Events.ResourcesWithdrawn, amount);
        return true;
    }
}
