using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour, IMenu
{
    public static MenuManager Instance { get; private set; }
    private AMenuState currentState;

    private Dictionary<string, GameObject> panels;

    private void Awake()
    {      
        Instance = this;
        if (panels == null) panels = new Dictionary<string, GameObject>();
        else panels.Clear();
        
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);

                if (!panels.ContainsKey(child.name))
                {
                    panels.Add(child.name, child.gameObject);
                    child.gameObject.SetActive(false);            
                }
            
        } 
    }

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "Menu")
        {
            SetState(new MainMenuState(this));
        }
        else
        {
            SetState(new GameState(this));
        }
    }

    // IMenu
    public GameObject GetGameObject() => gameObject;

    public IState GetState() => currentState;

    public void SetState(IState newState)
    {
        if (currentState != null)
            {
                currentState.Exit();    
            }
            
            currentState = (AMenuState)newState;
            currentState.Enter();
    }

     public GameObject GetPanel(string panelName)
    {
        if (!panels.TryGetValue(panelName, out GameObject panel))
        {
            return null;
        }
        return panel;
    }


    private void Update() => currentState?.Update();
    private void FixedUpdate() => currentState.FixedUpdate();

    // UI 
    // Se delega la accion a los estados y GameManager actúa como
    // contexto común para la gestion de botones
    public void OnPlay() => currentState.OnPlay();
    public void OnBack() => currentState.OnBack();
    public void OnSettings() => currentState.OnSettings();
    public void OnCredits() => currentState.OnCredits();

    public void OnMainMenu() => currentState.OnMainMenu();

    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}