using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates {GUARD, PATROL, CHASE, DEAD}
//直接挂载变量组件
[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(CharacterStates))]

public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private Collider coll;

    [Header("Basic Settings")]
    public float sightRadius;
    public bool isGuard;
    private float speed;
    protected GameObject attackTarget;
    public float lookAtTime;

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;
    private Vector3 guardPos;

    private float remainLookAtTime;
    private float lastAttackTime;
    private Quaternion guardRotation;
    protected CharacterStates characterStates;

    //bool配合动画
    bool isWalk;
    bool isChase;
    bool isFollow;
    bool isDead;
    bool playerDead;

    private void Awake() 
    {
        agent = GetComponent<NavMeshAgent>();   
        anim = GetComponent<Animator>();
        characterStates = GetComponent<CharacterStates>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        if(isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }

        // GameManager.Instance.AddObserver(this);
    }

    // 切换场景时启用
    void OnEnable()
    {
        GameManager.Instance.AddObserver(this);
    }

    void OnDisable()
    {
        if(!GameManager.IsInitialized) return;
        GameManager.Instance.RemoveObserver(this);

        if(GetComponent<LootSpawner>() && isDead)
            GetComponent<LootSpawner>().Spawnloot();

        if(QuestManager.IsInitialized && isDead)
        {
            Debug.Log(this.name);
            //TODO:敌人名称与任务目标名称配对（可以在characterData_SO中新增敌人名称以作辨别）
            QuestManager.Instance.UpdateQuestProgress(this.name, 1);
        }
    }

    private void Update()
    {
        if(characterStates.CurrentHealth == 0)
            isDead = true;

        if(!playerDead)
        {
            SwitchStates();   
            SwitchAnimation(); 
            lastAttackTime -= Time.deltaTime;
        }
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStates.isCritical);
        anim.SetBool("Death", isDead);
    }

    void SwitchStates()
    {
        if(isDead)
            enemyStates = EnemyStates.DEAD;

        //如果发现player，切换为CHASE
        else if(FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }

        switch(enemyStates)
        {
            case EnemyStates.GUARD:
                isChase = false;

                if(transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if(Vector3.SqrMagnitude(guardPos - transform.position) <= agent.stoppingDistance)
                        {
                            isWalk = false;
                            transform.rotation = Quaternion.Lerp(transform.rotation, guardRotation, 0.01f);
                        }
                }
                break;

            case EnemyStates.PATROL:
                isChase = false;
                agent.speed = speed * 0.5f;

                //判断是否到达随机地点
                if(Vector3.Distance(wayPoint, transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        if(remainLookAtTime > 0)
                            remainLookAtTime -= Time.deltaTime;
                        else
                            GetNewWayPoint();
                    }
                    else
                    {
                        isWalk = true;
                        agent.destination = wayPoint;
                    }
                break;

            case EnemyStates.CHASE:
                isWalk = false;
                isChase = true;

                speed = agent.speed;

                if(!FoundPlayer())
                {
                    isFollow = false;
                    if(remainLookAtTime > 0)
                    {
                        agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                        enemyStates = EnemyStates.GUARD;
                    else
                        enemyStates = EnemyStates.PATROL;
                }
                else
                {
                    isFollow = true;
                    agent.isStopped = false;
                    agent.destination = attackTarget.transform.position;
                }

                //判断攻击，技能距离
                if(TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;

                    //判断攻击cd
                    if(lastAttackTime < 0)
                    {
                        lastAttackTime = characterStates.attackData.coolDown;

                        //判断暴击
                        characterStates.isCritical = Random.value < characterStates.attackData.criticalChance;

                        //执行攻击
                        Attack();
                    }
                }

                break;

            case EnemyStates.DEAD:
                coll.enabled = false;
                //动画事件中不断检测agent，故敌人死亡后采用缩小radius处理报错
                // agent.enabled = false;
                agent.radius = 0;
                Destroy(gameObject, 2f);
                break;
        }
    }


    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if(TargetInAttackRange())
        {
            //近身攻击动画
            anim.SetTrigger("Attack");
        }

        if(TargetInSkillRange())
        {
            //技能攻击动画
            anim.SetTrigger("Skill");
        }
    }

    bool TargetInAttackRange()
    {
        if(attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStates.attackData.attackRange;
        else 
            return false;
    }

    bool TargetInSkillRange()
    {
        if(attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position, transform.position) <= characterStates.attackData.skillRange;
        else 
            return false;
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position, sightRadius);

        foreach(var target in colliders)
        {
            if(target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }
        attackTarget = null;

        return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange, patrolRange);
        float randomZ = Random.Range(-patrolRange, patrolRange);

        Vector3 randomPoint = new Vector3(guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1)? hit.position : transform.position;

    }

    void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, sightRadius); 
    }

    //Animation Event
    void Hit()
    {
        if(attackTarget != null && transform.IsFacingTarget(attackTarget.transform))
        {
            var targetStates = attackTarget.GetComponent<CharacterStates>();
            targetStates.TakeDamage(characterStates, targetStates);
        }
    }

    public void EndNotify()
    {
        anim.SetBool("Win",true);
        playerDead = true;
        isChase = false;
        isWalk = false;
        attackTarget = null;
    }
}
