using System.Security.Cryptography.X509Certificates;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class FlyweightEnemy
{
    public float sightRange = 5.0f;

    public int damage = 1;
 
    public float patrolPointRange = 10.0f;

    public float patrolTime;

    public float maxPatrolTime = 5.0f;

    public void Update()
    {
        patrolTime += Time.deltaTime;
    }
    public void IdlePatrol(Enemy enemy)
    {
        if (!enemy.patrolPointSet || patrolTime > maxPatrolTime) SetPatrolPoint(enemy);

        if(enemy.patrolPointSet) enemy.agent.SetDestination(enemy.patrolPoint);

        Vector3 patrolPointDistance = enemy.transform.position - enemy.patrolPoint;

        if(patrolPointDistance.magnitude < 1f) enemy.patrolPointSet = false;
    }
    public void SetPatrolPoint(Enemy enemy)
    {
        float randomZ = Random.Range(-patrolPointRange, patrolPointRange);
        float randomX = Random.Range(-patrolPointRange, patrolPointRange);

        enemy.patrolPoint = new Vector3(enemy.transform.position.x + randomX, enemy.transform.position.y, enemy.transform.position.z + randomZ);

        // Comprobación que el punto de destino este en el mapa
        if(Physics.Raycast(enemy.patrolPoint, -enemy.transform.up, 2f, enemy.Ground)) enemy.patrolPointSet = true;

        patrolTime = 0f;
    }

    public void ChasingPlayer(Enemy enemy)
    {
        enemy.agent.SetDestination(enemy.player.position);
    }
    public void Attack(Enemy enemy)
    {
        // 2. Lógica de Daño (Hitbox)
        if(enemy.inAttackRange)
        {
            Debug.Log("Enemigo golpea a Lumi ");
            //LumiController.TakeDamage();
        }
        
        enemy.hasAttacked = true;
        enemy.Invoke(nameof(ResetAttack), enemy.attackCooldown);
    }
    public void ResetAttack(Enemy enemy)
    {
        enemy.hasAttacked = false;
    }
    
    public void OnCollisionEnter(Collision collision, Enemy enemy)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LumiController player = collision.gameObject.GetComponent<LumiController>();

            if (player != null)
            {
                if (player.isInvincible)
                {
                    Debug.Log("¡El enemigo tocó a Lumi invencible y murió!");
                    Die(enemy);
                }
                else
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }
    public void Die(Enemy enemy)
    {
        enemy.Die();
    }
}
