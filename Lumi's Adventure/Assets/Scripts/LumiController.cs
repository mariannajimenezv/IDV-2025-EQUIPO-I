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
    private Animator anim; 
    private bool isGrounded;
    private float currentSpeed;

    [Header("Estado del Juego")]
    public int maxHealth = 10;
    public int currentHealth;

    // Lista de observadores (Patron Observer)
    private List<ILumiObserver> observers = new List<ILumiObserver>();

    [Header("Combate")]
    public Transform attackPoint;
    public float attackRange = 1.5f;
    public LayerMask enemyLayers;

    [Header("Power Ups")]
    public bool isInvincible = false;
    public float iFramesDuration = 1f;
    private LineRenderer moonGuideLine;
    private Coroutine iFramesCoroutine;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();

        anim = GetComponent<Animator>();
        if (anim == null) anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        currentHealth = maxHealth;
        currentSpeed = walkSpeed;

        moonGuideLine = GetComponent<LineRenderer>();
        if (moonGuideLine) moonGuideLine.enabled = false;

        NotifyObservers("Life", currentHealth);
    }

    private void Update()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        }

        if (anim != null)
        {
            anim.SetBool("IsGrounded", isGrounded);
        }
    }

    // --- METODOS DEL COMMAND PATTERN  ---


    public void Move(Vector3 direction)
    {
        if (anim != null)
        {
            anim.SetFloat("Speed", direction.magnitude);
        }

        if (direction.magnitude >= 0.1f)
        {
            Vector3 moveDir = direction * currentSpeed;

            rb.linearVelocity = new Vector3(moveDir.x, rb.linearVelocity.y, moveDir.z);

            transform.forward = direction;
        }
        else
        {
            rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0);
        }
    }

    public void Jump()
    {
        if (groundCheck != null)
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);
        }

        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            if (anim != null)
            {
                anim.SetTrigger("Jump");
            }

        }
    }

    public void Attack()
    {
        Debug.Log("Lumi ejecuta ataque");

        if (anim != null)
        {
            anim.SetTrigger("Attack");
        }

        if (attackPoint != null)
        {
            Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.gameObject == gameObject) continue;

                Debug.Log("Lumi golpea a: " + enemy.name);
                Destroy(enemy.gameObject);

                ServiceLocator.Get<IAudioService>().PlaySound("Attack");
            }
        }
    }

    // --- LOGICA DE JUEGO (DAMAGE, CURACION, POWERUPS) ---

    public void TakeDamage(int damage)
    {
        if (isInvincible)
        {
            Debug.Log("Lumi es invencible - Daño bloqueado");
            return;
        }

        currentHealth -= damage;
        NotifyObservers("Life", currentHealth);

        VisualFeedbackManager.Instance.ShowDamageFeedback(gameObject);    // Feedback visual (singleton)

        if (iFramesCoroutine != null)
        {
            StopCoroutine(iFramesCoroutine);
        }
        iFramesCoroutine = StartCoroutine(ActivateIFrames(iFramesDuration));

        ServiceLocator.Get<IAudioService>().PlaySound("Damage");    // Sonido

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

        VisualFeedbackManager.Instance.ShowHealFeedback(gameObject);    // Feedback visual (singleton)
        ServiceLocator.Get<IAudioService>().PlaySound("Heart");
    }

    public void CollectFragment(Transform fragment)
    {
        ServiceLocator.Get<IScoreService>().AddPoints();
        ServiceLocator.Get<IFragmentService>().UnregisterFragment(fragment);
        if(moonGuideLine != null) moonGuideLine.enabled = false;
        NotifyObservers("Fragment", ServiceLocator.Get<IScoreService>().CurrentScore);

        ServiceLocator.Get<IAudioService>().PlaySound("Fragment");
    }

    // --- POWER UPS ---

    public void CollectPowerUp(string type)
    {
        NotifyObservers("PowerUp", 0, type);

        ServiceLocator.Get<IAudioService>().PlaySound("PowerUp");

        if (type == "Star") StartCoroutine(ActivateInvincibility(5f));
        else if (type == "Moon" && moonGuideLine != null)
        {
            moonGuideLine.enabled = true;
            StartCoroutine(ActivateStarGuidance(10f));
        }
        else if (type == "Sun")
        {
            StartCoroutine(SpeedBoost(5f, 10f));
        }
    }

    public IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("�Lumi Invencible!");

        VisualFeedbackManager.Instance.ShowInvincibilityFeedback(gameObject, duration);    // Feedback visual (singleton)

        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Fin Invencibilidad");
    }

    public IEnumerator SpeedBoost(float duration, float boostedSpeed)
    {
        currentSpeed = boostedSpeed;

        VisualFeedbackManager.Instance.ShowSpeedBoostFeedback(gameObject, duration);    // Feedback visual (singleton)

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

    private IEnumerator ActivateIFrames(float duration)
    {
        isInvincible = true;
        Debug.Log($"i-frames activados por {duration} segundos");

        yield return new WaitForSeconds(duration);

        isInvincible = false;
        Debug.Log("i-frames terminados");
        iFramesCoroutine = null;
    }

    // --- PATRON OBSERVER ---

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
        foreach (ILumiObserver observer in new List<ILumiObserver>(observers))
        {
            if (eventType == "Life") observer.OnLifeChange(value);
            if (eventType == "Fragment") observer.OnFragmentCount(value);
            if (eventType == "PowerUp") observer.OnPowerUp(msg);
        }
    }
}