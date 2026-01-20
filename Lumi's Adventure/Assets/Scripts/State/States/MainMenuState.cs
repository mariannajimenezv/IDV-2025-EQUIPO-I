using UnityEngine;

/// <summary>
/// State for Main Menu Screen. First screen seen when launching
/// the game
/// </summary>
public class MainMenuState : AMenuState
{
    private readonly GameObject panel;
    private readonly GameObject background;

    // Constructor
    public MainMenuState(IMenu menu) : base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("MainMenuState");
        background = menu.GetPanel("MainMenuBg");
    }

    // State properties and transitions
    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        if (background != null) background.SetActive(true);
        Debug.Log("Entering Main Menu");
    }

    public override void Exit()
    {
        if (panel != null) panel.SetActive(false);
        Debug.Log("Exiting Main Menu");
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        
    }

    // BUTTON METHODS USED IN MAIN MENU //
    public override void OnPlay()
    {
        menu.SetState(new StartGameState(menu));
    }

    public override void OnTutorial()
    {
        menu.SetState(new TutorialState(menu));
    }

    public override void OnCredits()
    {
        menu.SetState(new CreditsState(menu));
    }

    public override void OnBack()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}