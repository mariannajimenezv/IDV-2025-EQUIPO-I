using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, IFlyweightEnemy
{

    public int health;                      
    public float attackRange;                   
    public float attackCooldown;                
    public bool hasAttacked;                    
    public Vector3 patrolPoint;                 
    public bool patrolPointSet;                        
    public bool inSightRange, inAttackRange;
    
    public bool isDead = false;
    public float deadTime = 0f;

    public Transform player;

    public NavMeshAgent agent;

    public LayerMask Ground, Player;

    private FlyweightEnemy fwEnemy;

    public Animator animator;


    public void Awake()
    {
        player = GameObject.Find("Lumi").transform;
        agent = GetComponent<NavMeshAgent>();
        fwEnemy = new FlyweightEnemy();
        animator = GetComponent<Animator>();
    }
    public void Update()
    {
        //Definición del rango de vision y ataque del enemigo
        inSightRange = Physics.CheckSphere(transform.position, fwEnemy.sightRange, Player);
        inAttackRange = Physics.CheckSphere(transform.position, attackRange, Player);

        Vector3 distanceToPlayer = transform.position - player.position;


        if (!inSightRange && !inAttackRange && !isDead) IdlePatrol();
        if (inSightRange && !inAttackRange && distanceToPlayer.magnitude > 0.5f && !isDead) ChasingPlayer();
        if (inSightRange && inAttackRange && !hasAttacked && !isDead) Attack();
        if (health < 0 || isDead) { deadTime += Time.deltaTime; Die(); }

        fwEnemy.Update();
    }

    public void IdlePatrol()
    {
        fwEnemy.IdlePatrol(this);
    }
    public void SetPatrolPoint()
    {
        fwEnemy.SetPatrolPoint(this);
    }

    public void ChasingPlayer()
    {
        fwEnemy.ChasingPlayer(this);
    }
    public void Attack()
    {
        fwEnemy.Attack(this);
    }
    public void ResetAttack()
    {
        fwEnemy.ResetAttack(this);
    }
    
    public void OnCollisionEnter(Collision collision)
    {
        fwEnemy.OnCollisionEnter(collision, this);
    }
    public void Die()
    {
        //if (deadTime < 1.5f) return;
        Destroy(gameObject);
    }
}
