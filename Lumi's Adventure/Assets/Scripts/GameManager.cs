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
    public GameObject portalObject; // El GameObject completo del portal
    public Collider portalCollider;
    public Color portalActiveColor = new Color(0f, 5f, 0f, 1f);
    public Color portalInactiveColor = new Color(0f, 0f, 0f, 1f);

    private Renderer[] portalRenderers; // Array para todos los renderers (LOD0 y LOD1)
    private bool portalUnlocked = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        if (exitDoor != null) exitDoor.SetActive(false);

        // Obtener TODOS los renderers del portal (incluye LOD0 y LOD1)
        if (portalObject != null)
        {
            portalRenderers = portalObject.GetComponentsInChildren<Renderer>();
            Debug.Log($"Portal: encontrados {portalRenderers.Length} renderers");
        }

        SetPortalActive(false);

        if (lumi != null)
        {
            lumi.AddObserver(this);
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
            GameObject fragment = ServiceLocator
                .Get<IItemFactory>()
                .CreateItem("Fragment", point.position);

            ServiceLocator
                .Get<IFragmentService>()
                .RegisterFragment(fragment.transform);
        }

        foreach (var point in powerSpawnPoints)
        {
            Debug.Log($"Spawning power-up with key: '{point.name}'");
            ServiceLocator
                .Get<IItemFactory>()
                .CreateItem(point.tag, point.position);
        }
    }

    private void OnDestroy()
    {
        if (lumi != null)
        {
            lumi.RemoveObserver(this);
        }
    }

    public void OnFragmentCount(int value)
    {
        currentFragments = value;
        Debug.Log($"[OBSERVER] Fragmentos: {currentFragments}/{totalFragments}");

        if (currentFragments >= totalFragments && !portalUnlocked)
        {
            UnlockExit();
        }
    }

    public void OnLifeChange(int value)
    {
        Debug.Log("[OBSERVER] Vida restante:" + value);
        if (value <= 0)
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

        SetPortalActive(true);
    }

    private void SetPortalActive(bool active)
    {
        if (portalRenderers == null || portalRenderers.Length == 0) return;

        float intensity = active ? 5f : 1f;
        Color targetColor = active ? portalActiveColor * intensity : portalInactiveColor;

        foreach (Renderer rend in portalRenderers)
        {
            if (rend == null) continue;

            Material[] mats = rend.materials;

            foreach (Material mat in mats)
            {
                if (mat.HasProperty("_Color"))
                {
                    mat.SetColor("_Color", targetColor);
                }

                if (mat.HasProperty("_EmissionColor"))
                {
                    mat.EnableKeyword("_EMISSION");
                    mat.SetColor("_EmissionColor", targetColor);
                }
            }
        }

        Debug.Log($"Portal actualizado a: {(active ? "ACTIVO (Verde Brillante)" : "INACTIVO (Negro)")}");
    }

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