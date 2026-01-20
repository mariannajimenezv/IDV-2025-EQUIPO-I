using UnityEngine;

public class GameState : AMenuState
{
    private readonly GameObject panel;

    // Constructor
    public GameState(IMenu menu): base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("GameState");
    }

    // State properties and transitions
    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 1; 
    }

    public override void Exit()
    {
        if (panel != null) panel.SetActive(false);
        Time.timeScale = 0;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            menu.SetState(new PauseState(menu));
        }
    }

    public override void FixedUpdate()
    {
        
    }
}