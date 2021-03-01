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
    AudioSource audioSource;
    AudioClip attackClip;
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
        audioSource = GetComponent<AudioSource>();
        attackClip = attackType.GetAudioClip();
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
        if (isAttacking && !canAttack)
        {
            StopAttack();
        }
        if (isAttacking && (!attackTarget || attackTarget.health.currentHealth == 0))
        {
            StopAttack();
        }

        if (showAttack)
        {
            animator.SetTrigger("shoot");
            animator.ResetTrigger("idle");
            animator.ResetTrigger("move");

            if (audioSource && !audioSource.isPlaying && attackType.attackSoundPath != "" && attackType.continuousSound) {
                audioSource.clip = attackClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        } else {
            if (audioSource && audioSource.isPlaying && audioSource.clip == attackClip) {
                audioSource.clip = null;
                audioSource.loop = false;
                audioSource.Stop();
            }
        }
        
        if (isAttacking)
        {
            transform.LookAt(new Vector3(attackTarget.transform.position.x, transform.position.y, attackTarget.transform.position.z));

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
