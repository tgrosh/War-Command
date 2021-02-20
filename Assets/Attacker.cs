using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : MonoBehaviour
{
    public AttackType attackType;

    Mover mover;
    Animator animator;
    Targeter targeter;
    Attackable attackTarget;
    bool canAttack;
    bool attacking;
    float attackTimer;

    // Start is called before the first frame update
    void Start()
    {
        mover = GetComponent<Mover>();
        targeter = GetComponent<Targeter>();
        animator = GetComponent <Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (targeter.target)
        {
            attackTarget = targeter.target.GetComponentInParent<Attackable>();
        }
        else
        {
            attackTarget = null;
        }

        canAttack = attackTarget && Vector3.Distance(transform.position, attackTarget.transform.position) <= attackType.range;

        if (!canAttack)
        {
            attacking = false;
        }

        if (attackTarget && canAttack)
        {
            Attack();
        }
        if (attackTarget && !canAttack)
        {
            MoveToTarget();
        }

        if (attacking)
        {
            attackTimer += attackType.attackSpeed * Time.deltaTime;

            if (attackTimer > attackType.attackSpeed)
            {
                attackTarget.Attack(attackType);
                attackTimer = 0f;
            }
        }
    }

    void Attack()
    {
        mover.ClearDestination();
        animator.SetTrigger("shoot");
        animator.ResetTrigger("idle");
        animator.ResetTrigger("move");
        attacking = true;
    }
    
    void MoveToTarget()
    {
        mover.SetDestination(attackTarget.transform.position);
        attacking = false;
    }
}
