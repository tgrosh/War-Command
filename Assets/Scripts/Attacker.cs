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
    bool showAttack;
    bool isAttacking;
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

        if (attackTarget && !canAttack)
        {
            MoveToTarget();
        }
        if (attackTarget && canAttack)
        {
            Attack();
        }
        if (!canAttack && isAttacking)
        {
            StopAttack();
        }
        if (!attackTarget && isAttacking)
        {
            StopAttack();
        }

        if (showAttack)
        {
            animator.SetTrigger("shoot");
            animator.ResetTrigger("idle");
            animator.ResetTrigger("move");
        }
        
        if (isAttacking) {
            attackTimer += attackType.attackSpeed * Time.deltaTime;

            if (attackTimer > attackType.attackSpeed)
            {
                CmdAttackTarget(attackTarget, attackType);
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
        isAttacking = true; //only local needs to actually perform the attack
        if (!showAttack) CmdSetShowAttack(true); // all clients need to know to show the attacking anims
    }

    [Command]
    void CmdAttackTarget(Attackable target, AttackType attackType)
    {
        target.Attack(attackType);
    }

    [Command]
    void CmdSetShowAttack(bool showAttack)
    {
        this.showAttack = showAttack;
    }

    void StopAttack()
    {
        animator.ResetTrigger("shoot");
        isAttacking = false;
        if (showAttack) CmdSetShowAttack(false);
    }
    
    void MoveToTarget()
    {
        mover.SetDestination(attackTarget.transform.position);
        isAttacking = false;
        if (showAttack) CmdSetShowAttack(false);
    }
}
