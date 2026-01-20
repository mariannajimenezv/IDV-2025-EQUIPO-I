using UnityEngine;

/// <summary>
/// State for Tutorial Menu. Shows player
/// basic controls for Lumi
/// </summary>
public class TutorialState : AMenuState
{
    private readonly GameObject panel;

    // Constructor
    public TutorialState(IMenu menu): base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("TutorialState");
    }

    // State properties and transitions
    public override void Enter()
    {
        Debug.Log("Entering Tutorial Menu");
        if (panel != null) panel.SetActive(true);
    }

    public override void Exit()
    {
        Debug.Log("Exiting Tutorial Menu");
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

    // BUTTON METHODS USED IN TUTORIAL MENU //
    public override void OnBack()
    {
        menu.SetState(new MainMenuState(menu));
    }
}
