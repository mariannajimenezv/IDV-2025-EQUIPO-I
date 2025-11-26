using UnityEngine;
using System.Collections;

public class LumiController : MonoBehaviour
{
    [Header("Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;
    public LayerMask groundLayer; 
    public Transform groundCheck; 

    private Rigidbody rb;
    private bool isGrounded;
    private float currentSpeed;

    [Header("Combate y Vida")]
    public int maxHealth = 100;
    public int currentHealth;
    public Transform attackPoint; 
    public float attackRange = 1.5f;
    public LayerMask enemyLayers; 

    [Header("Power Ups - Estados")]
    public bool isInvincible = false;

    private LineRenderer moonGuideLine;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        moonGuideLine = GetComponent<LineRenderer>();
        if (moonGuideLine) moonGuideLine.enabled = false;
    }

    void Update()
    {
        Move();
        Jump();

        if (Input.GetMouseButtonDown(0))
        {
            Attack();
        }
    }

    void Move()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); 
        float moveZ = Input.GetAxisRaw("Vertical");  

        if (Input.GetKey(KeyCode.LeftShift))
            currentSpeed = runSpeed;
        else
            currentSpeed = walkSpeed;

        Vector3 direction = new Vector3(moveX, 0f, moveZ).normalized;

        if (direction.magnitude >= 0.1f)
        {
            // mover personaje
            Vector3 moveDir = direction * currentSpeed;
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

            transform.forward = direction;
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    void Jump()
    {
        // detectar suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    void Attack()
    {
        Debug.Log("Lumi Ataca");
        // animaciones para luego:
        // animator.SetTrigger("Attack");

        // detectar enemigos en rango
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.gameObject == gameObject) continue;

            Debug.Log("lumi golpea a: " + enemy.name);
            Destroy(enemy.gameObject); // luego enemy.TakeDamage() para que tengan vida y baje y tal
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // si invencible, no daño

        currentHealth -= damage;
        Debug.Log("Vida actual: " + currentHealth);

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
    }

    // invencibilidad (estrella)
    public IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("Lumi: estado invencible");
        
        // activar efecto visual aquí
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Lumi: fin invencibilidad");
    }
}
