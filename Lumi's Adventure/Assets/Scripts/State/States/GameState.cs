using UnityEngine;

public class GameState : AMenuState
{
    private readonly GameObject panel;

    public GameState(IMenu menu): base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("CreditsMenuState");
    }

    public override void Enter()
    {
        GameObject panel = menu.GetPanel("GameState");
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 1; 
    }

    public override void Exit()
    {
        GameObject panel = menu.GetPanel("GameState");
        if (panel != null) panel.SetActive(false);
        Time.timeScale = 0;
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
        }
    }

    public override void FixedUpdate()
    {
        
    }
}