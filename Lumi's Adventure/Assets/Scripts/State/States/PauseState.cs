using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseState : AMenuState
{
    private readonly GameObject panel;

    // Constructor
    public PauseState(IMenu menu) : base(menu)
    {
        this.menu = menu;
        panel = menu.GetPanel("PauseState");
    }

    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0; 
    }

    public override void Exit()
    {
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetState(new GameState(menu));
        }
    }

    public override void FixedUpdate()
    {
        
    }

    // BUTTON METHODS USED IN PAUSE MENU //
    public override void OnBack()
    {
        Time.timeScale = 1; 
        menu.SetState(new GameState(menu));
    }

    public override void OnMainMenu()
    {
        Time.timeScale = 1; 
        SceneManager.LoadScene("Menu");
    }

}