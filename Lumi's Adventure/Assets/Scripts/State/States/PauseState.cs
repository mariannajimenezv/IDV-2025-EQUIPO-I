using UnityEngine;

public class PauseState : AMenuState
{

    public PauseState(IMenu menu) : base(menu)
    {
        this.menu = menu;
    }

    public override void Enter()
    {
        GameObject panel = menu.GetPanel("PauseMenuState");
        if (panel != null) panel.SetActive(true);
    }

    public override void Exit()
    {
        GameObject panel = menu.GetPanel("PauseMenuState");
        if (panel != null)
        {
            panel.SetActive(false);
        }
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Exit();
            menu.SetState(new GameState(menu));
        }
    }

    public override void FixedUpdate()
    {
        
    }

}