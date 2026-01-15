using UnityEngine;

public class PauseState : AMenuState
{
    private readonly GameObject panel;
    public PauseState(IMenu menu) : base(menu)
    {
        this.menu = menu;
        panel = menu.GetPanel("PauseState");
    }

    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
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

    public override void OnBack()
    {
        menu.SetState(new GameState(menu));
    }

    public override void OnMainMenu()
    {
        menu.SetState(new MainMenuState(menu));
    }

}