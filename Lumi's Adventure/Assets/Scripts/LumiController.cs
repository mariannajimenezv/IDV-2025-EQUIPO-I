using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LumiController : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;

    [Header("Físicas y Referencias")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody rb;
    private Animator anim; // Referencia para controlar las animaciones
    private bool isGrounded;
    private float currentSpeed;

    [Header("Estado del Juego")]
    public int maxHealth = 10;
    public int currentHealth;
    public int fragments = 0;

    // Lista de observadores (Patrón Observer)
    private List<ILumiObserver> observers = new List<ILumiObserver>();

    [Header("Combate")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;

    [Header("Power Ups")]
    public bool isInvincible = false;
    private LineRenderer moonGuideLine;

    private void Awake()
    {
        // 1. Obtener Rigidbody
        rb = GetComponent<Rigidbody>();

        // 2. Obtener Animator (Buscamos en hijos por si el modelo 3D está dentro)
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentSpeed = walkSpeed; // IMPORTANTE: Inicializar velocidad para que no sea 0

        // Configuración visual de la guía (Luna)
        moonGuideLine = GetComponent<LineRenderer>();
        if (moonGuideLine) moonGuideLine.enabled = false;

        NotifyObservers("Life", currentHealth);
    }

    // --- MÉTODOS DEL COMMAND PATTERN (PÚBLICOS) ---

    public void Move(Vector3 direction)
    {
        // 1. Control del Animator (Visual)
        if (anim != null)
        {
            // Si hay movimiento (magnitud > 0), pasamos el valor al Animator
            // Esto hará la transición de Idle -> Run
            anim.SetFloat("Speed", direction.magnitude);
        }

        // 2. Control Físico (Rigidbody)
        if (direction.magnitude >= 0.1f)
        {
            Debug.Log($"Moviendo. Dir: {direction} | Velocidad: {currentSpeed} | Resultado: {direction * currentSpeed}");
            // Calcular dirección y velocidad
            Vector3 moveDir = direction * currentSpeed;

            // Unity 6: Usamos linearVelocity
            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

            // Rotar al personaje hacia donde va
            transform.forward = direction;
        }
        else
        {
            // Frenar horizontalmente si no hay comando de movimiento
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void Jump()
    {
        // Detectar si tocamos suelo
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        }

        if (isGrounded)
        {
            // Impulso hacia arriba
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Si tuvieras animación de salto:
            // if(anim != null) anim.SetTrigger("Jump");
        }
    }

    public void Attack()
    {
        Debug.Log("Lumi ejecuta ataque");

        // 1. Activar Animación
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // 2. Lógica de Daño (Hitbox)
        if (attackPoint != null)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                // Evitamos que Lumi se golpee a sí misma
                if (enemy.gameObject == gameObject) continue;

                Debug.Log("Lumi golpea a: " + enemy.name);
                Destroy(enemy.gameObject);
            }
        }
    }

    // --- LÓGICA DE JUEGO (DAÑO, CURACIÓN, POWERUPS) ---

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        NotifyObservers("Life", currentHealth);

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        NotifyObservers("Life", currentHealth);
    }

    public void CollectFragment()
    {
        fragments++;
        NotifyObservers("Fragment", fragments);
    }

    // --- POWER UPS ---

    public void CollectPowerUp(string type)
    {
        NotifyObservers("PowerUp", 0, type);
        if (type == "Star") StartCoroutine(ActivateInvincibility(5f));
    }

    public IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("¡Lumi Invencible!");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Fin Invencibilidad");
    }

    // --- PATRÓN OBSERVER ---

    public void AddObserver(ILumiObserver observer)
    {
        if (!observers.Contains(observer)) observers.Add(observer);
    }

    public void RemoveObserver(ILumiObserver observer)
    {
        if (observers.Contains(observer)) observers.Remove(observer);
    }

    public void NotifyObservers(string eventType, int value = 0, string msg = "")
    {
        // Copiamos la lista para evitar errores si se modifica durante el bucle
        foreach (ILumiObserver observer in new List<ILumiObserver>(observers))
        {
            if (eventType == "Life") observer.OnLifeChange(value);
            if (eventType == "Fragment") observer.OnFragmentCount(value);
            if (eventType == "PowerUp") observer.OnPowerUp(msg);
        }
    }
}