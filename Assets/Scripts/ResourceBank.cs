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

    public static bool Withdraw(int ironAmount, int oilAmount)
    {
        if (instance.currentIron < ironAmount) return false;
        if (instance.currentOil < oilAmount) return false;

        instance.currentIron -= ironAmount;
        instance.currentOil -= oilAmount;
        EventManager.Emit(EventManager.EventMessage.IronAmountChanged, instance.currentIron);
        EventManager.Emit(EventManager.EventMessage.OilAmountChanged, instance.currentOil);

        return true;
    }
}
