using UnityEngine;

public class SettingsMenuState : AMenuState
{
    private readonly GameObject panel;

    public SettingsMenuState(IMenu menu) : base(menu)
    {
        panel = menu.GetPanel("SettingsMenuState");
    }

    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        Debug.Log("Entering Settings Menu");
    }

    public override void Exit()
    {
        if (panel != null) panel.SetActive(false);
        Debug.Log("Exiting Settings Menu");
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
           menu.SetState(new MainMenuState(menu));
        }
    }

    public override void FixedUpdate()
    {

    }

    public override void OnBack()
    {
        menu.SetState(new MainMenuState(menu));
    }

}