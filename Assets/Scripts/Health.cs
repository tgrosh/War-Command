using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SyncVar]
    public int currentHealth;
    [SyncVar]
    public int maxHealth;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    public void Damage(int amount)
    {
        if (currentHealth >= amount)
        {
            currentHealth -= amount;
        } else
        {
            currentHealth = 0;
        }
    }

    public void Heal(int amount)
    {
        if (currentHealth + amount <= maxHealth)
        {
            currentHealth += amount;
        } else
        {
            currentHealth = maxHealth;
        }
    }
}
