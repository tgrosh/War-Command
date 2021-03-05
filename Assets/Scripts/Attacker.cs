using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attacker : NetworkBehaviour
{
    public AttackType attackType;
    public float guardRadius = 10f;

    Mover mover;
    Animator animator;
    Attackable attackTarget;
    ActionQueue queue;
    Action currentAction;
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
        animator = GetComponent <Animator>();
        audioSource = GetComponent<AudioSource>();
        attackClip = attackType.GetAudioClip();
        queue = GetComponent<ActionQueue>();
    }

    // Update is called once per frame
    void Update()
    {
        if (queue.Peek() != null && queue.Peek().actionType == ActionType.Attack)
        {
            currentAction = queue.Peek();
        }
        else
        {
            currentAction = null;
        }

        if (currentAction != null)
        {
            attackTarget = currentAction.actionTarget.GetComponentInParent<Attackable>();
        }
        else
        {
            attackTarget = null;
        }

        canAttack = attackTarget && Vector3.Distance(transform.position, attackTarget.transform.position) <= attackType.range;

        if (hasAuthority && attackTarget && !canAttack)
        {
            MoveToTarget();
        }
        if (hasAuthority && attackTarget && canAttack)
        {
            Attack();
        }
        if (hasAuthority && isAttacking && !canAttack)
        {
            StopAttack();
        }
        if (hasAuthority && isAttacking && (!attackTarget || attackTarget.health.currentHealth == 0))
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
            animator.ResetTrigger("shoot");
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
            }
        }

        if (hasAuthority && !isAttacking && !attackTarget) {
            // look for a target
            foreach (Collider collider in Physics.OverlapSphere(transform.position, guardRadius)) {
                Attackable attackable = collider.GetComponentInParent<Attackable>();
                if (attackable && attackable.health.currentHealth > 0 && !attackable.GetComponentInParent<NetworkIdentity>().hasAuthority) {
                    queue.Clear();
                    queue.Add(new Action(ActionType.Attack, attackable.gameObject));
                }
            }
        }
    }

    void Attack()
    {
        mover.ClearDestination();
        isAttacking = true; //only local needs to actually perform the attack
        if (!showAttack) CmdSetShowAttack(true); // all clients need to know to show the attacking anims
        showAttack = true;
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
        queue.Next();
        isAttacking = false;
        currentAction = null;
        attackTarget = null;
        if (showAttack) CmdSetShowAttack(false);
        showAttack = false;
    }
    
    void MoveToTarget()
    {
        mover.SetDestination(attackTarget.transform);
        isAttacking = false;
        if (showAttack) CmdSetShowAttack(false);
        showAttack = false;
    }
}
