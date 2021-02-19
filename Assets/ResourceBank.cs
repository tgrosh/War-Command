using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBank : MonoBehaviour
{
    public int currentResources;
    public int startingResources = 200;

    private static ResourceBank resourceBank;
    public static ResourceBank instance
    {
        get
        {
            if (!resourceBank)
            {
                resourceBank = FindObjectOfType(typeof(ResourceBank)) as ResourceBank;
            }

            return resourceBank;
        }
    }

    private void Start()
    {
        instance.currentResources = startingResources;
        EventManager.Emit(EventManager.Events.ResourceAmountChanged, instance.currentResources);
    }

    public static int GetCurrentResources()
    {
        return instance.currentResources;
    }

    public static void Deposit(int amount)
    {
        instance.currentResources += amount;
        EventManager.Emit(EventManager.Events.ResourceAmountChanged, instance.currentResources);
    }

    public static bool Withdraw(int amount)
    {
        if (instance.currentResources < amount) return false;

        instance.currentResources -= amount;
        EventManager.Emit(EventManager.Events.ResourceAmountChanged, instance.currentResources);
        return true;
    }
}
