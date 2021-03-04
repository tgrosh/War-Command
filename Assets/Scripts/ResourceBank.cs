using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceBank : MonoBehaviour
{
    public int currentIron;
    public int currentOil;
    public int startingIron = 200;
    public int startingOil = 200;

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
        instance.currentIron = startingIron;
        instance.currentOil = startingOil;
        EventManager.Emit(EventManager.EventMessage.IronAmountChanged, instance.currentIron);
        EventManager.Emit(EventManager.EventMessage.OilAmountChanged, instance.currentOil);
    }

    public static void DepositIron(int amount)
    {
        instance.currentIron += amount;
        EventManager.Emit(EventManager.EventMessage.IronAmountChanged, instance.currentIron);
    }

    public static void DepositOil(int amount)
    {
        instance.currentOil += amount;
        EventManager.Emit(EventManager.EventMessage.OilAmountChanged, instance.currentOil);
    }

    public static bool WithdrawIron(int amount)
    {
        if (instance.currentIron < amount) return false;

        instance.currentIron -= amount;
        EventManager.Emit(EventManager.EventMessage.IronAmountChanged, instance.currentIron);
        return true;
    }

    public static bool WithdrawOil(int amount)
    {
        if (instance.currentOil < amount) return false;

        instance.currentOil -= amount;
        EventManager.Emit(EventManager.EventMessage.OilAmountChanged, instance.currentOil);
        return true;
    }
}
