using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
    public AttackType attackType;

    Mover mover;
    Animator animator;
    Targeter targeter;
    Attackable attackTarget;
    bool canAttack;
    [SyncVar]
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

        if (!canAttack && attacking)
        {
            CmdSetAttacking(false);
        }

        if (attackTarget && canAttack)
        {
            Attack();
        }
        if (attackTarget && !canAttack)
        {
            MoveToTarget();
        }
        if (!attackTarget && attacking)
        {
            StopAttack();
        }

        if (attacking)
        {
            animator.SetTrigger("shoot");
            animator.ResetTrigger("idle");
            animator.ResetTrigger("move"); 
            transform.LookAt(new Vector3(attackTarget.transform.position.x, transform.position.y, attackTarget.transform.position.z));

            attackTimer += attackType.attackSpeed * Time.deltaTime;

            if (attackTimer > attackType.attackSpeed)
            {
                attackTarget.Attack(attackType);
                attackTimer = 0f;

                if (attackTarget.health.currentHealth == 0)
                {
                    StopAttack();
                }
            }
        }
    }

    void Attack()
    {
        mover.ClearDestination();
        if (!attacking) CmdSetAttacking(true);
    }

    [Command]
    void CmdSetAttacking(bool isAttacking)
    {
        attacking = isAttacking;
    }

    void StopAttack()
    {
        animator.ResetTrigger("shoot");
        if (attacking) CmdSetAttacking(false);
    }
    
    void MoveToTarget()
    {
        mover.SetDestination(attackTarget.transform.position);
        if (attacking) CmdSetAttacking(false);
    }
}
