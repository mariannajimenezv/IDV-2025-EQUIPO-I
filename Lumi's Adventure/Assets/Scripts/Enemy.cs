using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Configuración")]
    public float speed = 3f;
    public int damage = 20;

    [Header("Ruta de Patrulla")]
    public Transform[] patrolPoints;
    private int currentPointIndex = 0;

    void Update()
    {
        Patrol();
    }

    void Patrol()
    {
        if (patrolPoints.Length == 0) return;

        Transform target = patrolPoints[currentPointIndex];

        transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);

        transform.LookAt(new Vector3(target.position.x, transform.position.y, target.position.z));

        if (Vector3.Distance(transform.position, target.position) < 0.2f)
        {
            currentPointIndex = (currentPointIndex + 1) % patrolPoints.Length;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            LumiController player = collision.gameObject.GetComponent<LumiController>();

            if (player != null)
            {
                if (player.isInvincible)
                {
                    Debug.Log("¡El enemigo tocó a Lumi invencible y murió!");
                    Die();
                }
                else
                {
                    player.TakeDamage(damage);
                }
            }
        }
    }

    public void Die()
    {
        Destroy(gameObject);
    }
}
