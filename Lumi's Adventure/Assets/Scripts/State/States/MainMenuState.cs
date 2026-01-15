using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuState : AMenuState
{
    private readonly GameObject panel;
    private readonly GameObject background;
    private readonly GameObject brightness;

    public MainMenuState(IMenu menu) : base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("MainMenuState");
        background = menu.GetPanel("MainMenuBg");
        brightness = menu.GetPanel("Brightness");
    }


    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        if (background != null) background.SetActive(true);
         if (brightness != null) brightness.SetActive(true);
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

    public override void OnPlay()
    {
        SceneManager.LoadScene("Game");
    }

    public override void OnCredits()
    {
        menu.SetState(new CreditsState(menu));
    }

    public override void OnSettings()
    {
        menu.SetState(new SettingsMenuState(menu));
    }

    public override void OnBack()
    {
        Debug.Log("Quitting game");
        Application.Quit();
    }
}