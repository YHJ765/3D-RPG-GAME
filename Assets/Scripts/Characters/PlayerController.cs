using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerController : MonoBehaviour
{
    private NavMeshAgent agent;

    private Animator anim;

    private GameObject attackTarget;
    private CharacterStates characterStates;
    private float lastAttackTime;
    private bool isDead;

    private float stopDistance;

    void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();   
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();

        stopDistance = agent.stoppingDistance;
    }
    
    void OnEnable() 
    {
        MouseManager.Instance.OnMouseClicked += MoveToTarget; 
        MouseManager.Instance.OnEnemyClicked += EventAttack;   
        GameManager.Instance.RigisterPlayer(characterStates);
    }

    void Start() 
    {
        SaveManager.Instance.LoadPlayerData();
    }

    void OnDisable() 
    {
        if(!MouseManager.IsInitialized) return;
        MouseManager.Instance.OnMouseClicked -= MoveToTarget; 
        MouseManager.Instance.OnEnemyClicked -= EventAttack;       
    }

    void Update() 
    {
        //符合返回true，不符合返回false
        isDead = characterStates.CurrentHealth == 0;

        if(isDead)
            GameManager.Instance.NotifyObservers();

        SwitchAnimation();

        lastAttackTime -= Time.deltaTime;
    }

    private void SwitchAnimation()
    {
        anim.SetFloat("Speed", agent.velocity.sqrMagnitude);
        anim.SetBool("Death", isDead);
    }

    public void MoveToTarget(Vector3 target)
    {
        StopAllCoroutines();
        if(isDead) return;

        agent.stoppingDistance = stopDistance;
        agent.isStopped = false;
        agent.destination = target;
    }

    private void EventAttack(GameObject target)
    {
        if(isDead) return;

        if(target != null)
        {
            attackTarget = target;
            characterStates.isCritical = UnityEngine.Random.value < characterStates.attackData.criticalChance;
            StartCoroutine(MoveToAttackTarget());
        }
    }

    IEnumerator MoveToAttackTarget()
    {
        agent.isStopped = false;
        agent.stoppingDistance = characterStates.attackData.attackRange;

        transform.LookAt(attackTarget.transform);
        while(Vector3.Distance(attackTarget.transform.position, transform.position) > characterStates.attackData.attackRange)
        {
            agent.destination = attackTarget.transform.position;
            yield return null;
        }

        agent.isStopped = true;

        //Attack
        if(lastAttackTime < 0)
        {
            anim.SetBool("Critical", characterStates.isCritical);
            anim.SetTrigger("Attack");
            //重制冷却时间
            lastAttackTime = characterStates.attackData.coolDown;
        }
    }

    //Animation Event
    void Hit()
    {
        if(attackTarget.CompareTag("Attackable"))
        {
            if(attackTarget.GetComponent<Rock>() && attackTarget.GetComponent<Rock>().rockStates == Rock.RockStates.HitNothing)
            {
                attackTarget.GetComponent<Rock>().rockStates = Rock.RockStates.HitEnemy;
                attackTarget.GetComponent<Rigidbody>().velocity = Vector3.one;
                attackTarget.GetComponent<Rigidbody>().AddForce(transform.forward * 20, ForceMode.Impulse);
            }
        }
        else
        {
            var targetStates = attackTarget.GetComponent<CharacterStates>();

            targetStates.TakeDamage(characterStates, targetStates);
        }
    }
}
