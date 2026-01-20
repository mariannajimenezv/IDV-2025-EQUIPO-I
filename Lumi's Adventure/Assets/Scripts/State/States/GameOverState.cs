using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverState : AMenuState
{
    private readonly GameObject panel;

    // Constructor
    public GameOverState(IMenu menu): base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("GameOverState");
    }

    // State properties and transitions
    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0; 
    }

    public override void Exit()
    {
        // The State Machine "dies" here so its not neccesary to do anything
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        
    }

    // BUTTON METHODS USED IN GAME OVER MENU //
    public override void OnBack()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Game");
    }

    public override void OnMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}