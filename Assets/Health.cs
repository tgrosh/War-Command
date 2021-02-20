using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int currentHealth;
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
        }
    }

    public void Heal(int amount)
    {
        if (currentHealth + amount <= maxHealth)
        {
            currentHealth += amount;
        }
    }
}
