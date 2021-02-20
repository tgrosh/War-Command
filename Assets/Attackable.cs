using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attackable : MonoBehaviour
{
    Health health;
    Animator animator;
    bool isDead;

    // Start is called before the first frame update
    void Start()
    {
        health = GetComponent<Health>();
        animator = GetComponent<Animator>(); 
        if (!animator)
        {
            animator = gameObject.AddComponent(typeof(Animator)) as Animator;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            animator.SetTrigger("die");
            isDead = true;
            Destroy(gameObject, 2f);
        }
    }

    public void Attack(int amount)
    {
        if (health)
        {
            health.Damage(amount);
        }
    }
}
