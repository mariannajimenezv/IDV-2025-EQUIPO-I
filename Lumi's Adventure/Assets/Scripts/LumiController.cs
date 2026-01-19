using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LumiController : MonoBehaviour
{
    [Header("Configuraci�n de Movimiento")]
    public float walkSpeed = 5f;
    public float runSpeed = 10f;
    public float jumpForce = 7f;
    public float gravityMultiplier = 2.5f; 

    [Header("F�sicas y Referencias")]
    public LayerMask groundLayer;
    public Transform groundCheck;

    private Rigidbody rb;
    private Animator anim; // Referencia para controlar las animaciones
    private bool isGrounded;
    private float currentSpeed;

    [Header("Estado del Juego")]
    public int maxHealth = 10;
    public int currentHealth;

    // Lista de observadores (Patr�n Observer)
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

        // 2. Obtener Animator (Buscamos en hijos por si el modelo 3D est� dentro)
        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentSpeed = walkSpeed; // IMPORTANTE: Inicializar velocidad para que no sea 0

        // Configuraci�n visual de la gu�a (Luna)
        moonGuideLine = GetComponent<LineRenderer>();
        if (moonGuideLine) moonGuideLine.enabled = false;

        NotifyObservers("Life", currentHealth);
    }

    private void Update()
    {
        // 1. Detectar suelo constantemente (haya input o no)
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        }

        // 2. Enviar el dato al Animator para cortar el salto al aterrizar
        if (anim != null)
        {
            anim.SetBool("IsGrounded", isGrounded);
        }
    }

    // --- M�TODOS DEL COMMAND PATTERN (P�BLICOS) ---


    public void Move(Vector3 direction)
    {
        // 1. Control del Animator (Visual)
        if (anim != null)
        {
            // Si hay movimiento (magnitud > 0), pasamos el valor al Animator
            // Esto har� la transici�n de Idle -> Run
            anim.SetFloat("Speed", direction.magnitude);
        }

        // 2. Control F�sico (Rigidbody)
        if (direction.magnitude >= 0.1f)
        {
            // Calcular direcci�n y velocidad
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
            // 1. Impulso fisico
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            //Activar animaci�n
            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }

        }
    }

    public void Attack()
    {
        Debug.Log("Lumi ejecuta ataque");

        // 1. Activar Animaci�n
        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        // 2. L�gica de Da�o (Hitbox)
        if (attackPoint != null)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                // Evitamos que Lumi se golpee a s� misma
                if (enemy.gameObject == gameObject) continue;

                Debug.Log("Lumi golpea a: " + enemy.name);
                Destroy(enemy.gameObject);

                //SI LE VAN A AGREGAR SONIDO
                //ServiceLocator.Get<IAudioService>().PlaySound("LumiHit");
            }
        }
    }

    // --- L�GICA DE JUEGO (DA�O, CURACI�N, POWERUPS) ---

    public void TakeDamage(int damage)
    {
        if (isInvincible) return;

        currentHealth -= damage;
        NotifyObservers("Life", currentHealth);

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver();
        }

        //SI LE VAN A AGREGAR SONIDO
        //ServiceLocator.Get<IAudioService>().PlaySound("LumiHit");

    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;
        NotifyObservers("Life", currentHealth);
        //SI LE VAN A AGREGAR SONIDO
        //ServiceLocator.Get<IAudioService>().PlaySound("LumiHit");

    }

    public void CollectFragment()
    {
        ServiceLocator.Get<IScoreService>().AddPoints();
        NotifyObservers("Fragment", ServiceLocator.Get<IScoreService>().CurrentScore);
        //SI LE VAN A AGREGAR SONIDO
        //ServiceLocator.Get<IAudioService>().PlaySound("LumiHit");

    }

    // --- POWER UPS ---

    public void CollectPowerUp(string type)
    {
        NotifyObservers("PowerUp", 0, type);

        //SI LE VAN A AGREGAR SONIDO
        //ServiceLocator.Get<IAudioService>().PlaySound("LumiHit");

        if (type == "Star") StartCoroutine(ActivateInvincibility(5f));
        else if (type == "Moon" && moonGuideLine != null)
        {
            moonGuideLine.enabled = true;
            StartCoroutine(ActivateStarGuidance(10f));
        }
        else if (type == "Sun")
        {
            // Ejemplo: Aumentar velocidad temporalmente
            StartCoroutine(SpeedBoost(5f, 10f));
        }
    }

    public IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("�Lumi Invencible!");
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Fin Invencibilidad");
    }

    public IEnumerator SpeedBoost(float duration, float boostedSpeed)
    {
        currentSpeed = boostedSpeed;
        yield return new WaitForSeconds(duration);
        currentSpeed = walkSpeed;
    }

    private IEnumerator ActivateStarGuidance(float duration)
    {
        moonGuideLine.enabled = true;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            Transform nearest = ServiceLocator
                .Get<IFragmentService>()
                .GetNearestFragment(transform.position);

            if (nearest)
            {
                moonGuideLine.SetPosition(0, transform.position);
                moonGuideLine.SetPosition(1, nearest.position);
            }
            else
            {
                moonGuideLine.enabled = false;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        moonGuideLine.enabled = false;
    }


    // --- PATR�N OBSERVER ---

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