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

    private void Awake()
    {
        if (Instance == null) Instance = this; //singleton
        else Destroy(gameObject);
    }

    private void Start()
    {
        // inicio salida bloqueada
        if (exitDoor != null) exitDoor.SetActive(false);

        if(lumi != null)
        {
            lumi.AddObserver(this); // patron observer
        }
        else
        {
            Debug.LogError("GameManager: ¡Falta asignar a Lumi en el Inspector!");
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
        if (currentFragments >= totalFragments)
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
        Debug.Log("SALIDA DESBLOQUEADA!!");
        if (exitDoor != null) exitDoor.SetActive(true);
        // maybe sonidito de completar nivel
    }

    public void GameOver()
    {
        Debug.Log("GAME OVER");
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.SetState(new GameOverState(MenuManager.Instance));
        }
    }

    public void WinLevel()
    {
        Debug.Log("NIVEL COMPLETADO!!");
        // siguiente nivel (en la entrega pone que con uno vale)
        if (MenuManager.Instance != null)
        {
            MenuManager.Instance.SetState(new VictoryState(MenuManager.Instance));
        }
    }
}
