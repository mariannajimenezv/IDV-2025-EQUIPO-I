using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

    [Header("Estado del juego")]    // Patron observer
    public int lives = 10;  
    public int fragments = 0;

    // Guardamos los suscriptores - del patron observer
    private List<ILumiObserver> observers = new List<ILumiObserver>();

    [Header("Combate y Vida")]
    public int maxHealth = 10;
    public int currentHealth;
    public Transform attackPoint; 
    public float attackRange = 1.5f;
    public LayerMask enemyLayers; 

    [Header("Power Ups - Estados")]
    public bool isInvincible = false;

    private LineRenderer moonGuideLine;

    private void Awake()
    {
        // Esto conecta la variable 'rb' con el componente real del objeto
        rb = GetComponent<Rigidbody>();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentHealth = maxHealth;

        currentSpeed = walkSpeed;

        moonGuideLine = GetComponent<LineRenderer>();
        if (moonGuideLine) moonGuideLine.enabled = false;

        NotifyObservers("life", currentHealth);     // notifica a los observers el estado inicial
    }

    void Update()   //patron observer
    {
        NotifyObservers("life", currentHealth);     // notifica a los observers el estado inicial
    }

    public void Move(Vector3 direction)
    { 
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

    public void Jump()
    {
        // detectar suelo
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            #region Service Locator
            #endregion
        }
    }

    public void Attack()
    {
        Debug.Log("Lumi Ataca");
        // animaciones para luego:
        // animator.SetTrigger("Attack");

        // detectar enemigos en rango
        Collider[] hitEnemies = Physics.OverlapSphere(attackPoint.position, attackRange, enemyLayers);

        foreach (Collider enemy in hitEnemies)
        {
            if (enemy.gameObject == gameObject) continue;
            #region Service Locator
            #endregion

            Debug.Log("lumi golpea a: " + enemy.name);
            Destroy(enemy.gameObject); // luego enemy.TakeDamage() para que tengan vida y baje y tal
        }
    }

    public void TakeDamage(int damage)
    {
        if (isInvincible) return; // si invencible, no da?o

        currentHealth -= damage;
        Debug.Log("Vida actual: " + currentHealth);

        NotifyObservers("Life", currentHealth);     // Notificamos a los observadores del da?o recibido

        if (currentHealth <= 0)
        {
            GameManager.Instance.GameOver(); 
        }
    }

    public void Heal(int amount)
    {
        currentHealth += amount;
        if (currentHealth > maxHealth) currentHealth = maxHealth;

        NotifyObservers("Life", currentHealth);     // Notificamos a los observadores si nos curamos
    }

    public void CollectFragment()
    {
        fragments++;
        NotifyObservers("Fragment", fragments);     // Notificamos a los observadores de los fragmentos recogidos
        #region Service Locator
        #endregion
    }

    public void CollectPowerUp(string type)
    {
        NotifyObservers("PowerUp", 0, type);     // Notificamos a los observadores del power up recogido
        if (type == "Star") StartCoroutine(ActivateInvincibility(5f));
    }


    // invencibilidad (estrella)
    public IEnumerator ActivateInvincibility(float duration)
    {
        isInvincible = true;
        Debug.Log("Lumi: estado invencible");
        
        // activar efecto visual aqu?
        yield return new WaitForSeconds(duration);
        isInvincible = false;
        Debug.Log("Lumi: fin invencibilidad");
    }

    // _-. PATRON OBSERVER .-_ \\
    public void AddObserver(ILumiObserver observer)
    {
        if (!observers.Contains(observer))
            observers.Add(observer);
    }

    public void RemoveObserver(ILumiObserver observer)
    {
        if (observers.Contains(observer))
            observers.Remove(observer);
    }

    public void NotifyObservers(string eventType, int value = 0, string msg = "")
    {
        foreach(ILumiObserver observer in observers)
        {
            if (eventType == "Life") observer.OnLifeChange(value);
            if (eventType == "Fragment") observer.OnFragmentCount(value);
            if (eventType == "PowerUp") observer.OnPowerUp(msg);

        }
    }
}
