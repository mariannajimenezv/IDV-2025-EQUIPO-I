using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour, ILumiObserver
{
    public static GameManager Instance;

    [Header("Referencias Patron Observer")]
    public LumiController lumi;

    [Header("Configuracion del Nivel")]
    public int totalFragments = 10;
    public int currentFragments = 0;

    public GameObject exitDoor;
    public Transform[] fragSpawnPoints;
    public Transform[] powerSpawnPoints;

    [Header("Portal de Salida")]
    public Renderer portalRenderer; // Renderer con el shader verde
    public Collider portalCollider; // Collider del portal (opcional)
    public Color portalActiveColor = new Color(0f, 1f, 0f, 1f); // Verde brillante
    public Color portalInactiveColor = new Color(0f, 0f, 0f, 1f); // Negro (apagado)

    private bool portalUnlocked = false;

    private void Awake()
    {
        if (Instance == null) Instance = this; //singleton
        else Destroy(gameObject);
    }

    private void Start()
    {
        // Inicio salida bloqueada
        if (exitDoor != null) exitDoor.SetActive(false);

        // Apagar portal al inicio
        SetPortalActive(false);

        if (lumi != null)
        {
            lumi.AddObserver(this); // patron observer
        }
        else
        {
            Debug.LogError("GameManager: ¡Falta asignar a Lumi en el Inspector!");
        }

        StartLevel();
        ServiceLocator.Get<IAudioService>().PlaySound("Game");
    }

    private void StartLevel()
    {
        foreach (var point in fragSpawnPoints)
        {
            ServiceLocator
                .Get<IItemFactory>()
                .CreateItem("Fragment", point.position);
            ServiceLocator
                .Get<IFragmentService>()
                .RegisterFragment(point);
        }

        foreach (var point in powerSpawnPoints)
        {
            Debug.Log($"Spawning power-up with key: '{point.name}'");
            ServiceLocator
                .Get<IItemFactory>()
                .CreateItem(point.name, point.position);
        }
    }

    private void OnDestroy()
    {
        if (lumi != null)
        {
            lumi.RemoveObserver(this); // patron observer
        }
    }

    public void OnFragmentCount(int value)
    {
        currentFragments = value;
        Debug.Log($"[OBSERVER] Fragmentos: {currentFragments}/{totalFragments}");

        // logica de victoria
        if (currentFragments >= totalFragments && !portalUnlocked)
        {
            UnlockExit();
        }
    }

    public void OnLifeChange(int value)
    {
        Debug.Log("[OBSERVER] Vida restante:" + value);

        if(value <= 0)
        {
            GameOver();
        }
    }

    public void OnPowerUp(string value)
    {
        Debug.Log("[OBSERVER] PowerUp activado" + value);
    }

    void UnlockExit()
    {
        portalUnlocked = true;
        Debug.Log("¡SALIDA DESBLOQUEADA!");

        if (exitDoor != null) exitDoor.SetActive(true);

        // ACTIVAR PORTAL (SHADER VERDE)
        SetPortalActive(true);
    }

    // Controla el estado visual del portal
    private void SetPortalActive(bool active)
    {
        if (portalRenderer != null)
        {
            // Cambiar color del shader
            Color targetColor = active ? portalActiveColor : portalInactiveColor;

            if (portalRenderer.material.HasProperty("_Color"))
            {
                portalRenderer.material.SetColor("_Color", targetColor);
            }
            else if (portalRenderer.material.HasProperty("Color"))
            {
                portalRenderer.material.SetColor("Color", targetColor);
            }

            Debug.Log($"Portal color cambiado a: {(active ? "Verde (activo)" : "Negro (inactivo)")}");
        }

        // Activar/desactivar collider para evitar que entren antes de tiempo
        if (portalCollider != null)
        {
            portalCollider.enabled = active;
        }
    }

    // Llamar cuando el jugador entra al portal
    public void OnPlayerEnterPortal()
    {
        if (!portalUnlocked)
        {
            Debug.Log("Portal bloqueado. Recoge todos los fragmentos primero.");
            return;
        }

        WinLevel();
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");

        ServiceLocator.Get<IAudioService>().PlaySound("GameOver");

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.SetState(new GameOverState(MenuManager.Instance));
        }
    }

    public void WinLevel()
    {
        Debug.Log("NIVEL COMPLETADO!!");

        ServiceLocator.Get<IAudioService>().PlaySound("Victory");

        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.SetState(new VictoryState(MenuManager.Instance));
        }
    }
}
