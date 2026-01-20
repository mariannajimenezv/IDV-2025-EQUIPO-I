using UnityEngine;

/// <summary>
/// State for Credits Menu. Accessed through Main Menu
/// </summary>
public class CreditsState : AMenuState
{
    private readonly GameObject panel;

    // Constructor
    public CreditsState(IMenu menu): base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("CreditsMenuState");
    }

    // State properties and transitions
    public override void Enter()
    {
        Debug.Log("Entering Credits Menu");
        if (panel != null) panel.SetActive(true);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Credits Menu");
        if (panel != null) panel.SetActive(false);
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

    // BUTTON METHODS USED IN CREDITS MENU //
    public override void OnBack()
    {
        menu.SetState(new MainMenuState(menu));
    }
}