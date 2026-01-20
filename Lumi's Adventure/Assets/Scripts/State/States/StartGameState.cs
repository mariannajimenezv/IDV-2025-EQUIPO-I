using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// State for Start Game Menu. When pressing PLay on Main
/// Menu, this state serves as context for the game lore.
/// </summary>
public class StartGameState : AMenuState
{
    private readonly GameObject panel;
    private List<GameObject> frames;
    private int frameCount;

    // Constructor
    public StartGameState(IMenu menu) : base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("GameStartState");
    }

    // State properties and transitions
    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        Debug.Log("Entering Start Game Menu");

        frames = panel.transform.Cast<Transform>()
            .Where(c => c.name.Contains("Frame"))
            .Select(c => c.gameObject).ToList();
        frameCount = 0; 
        frames?[frameCount].SetActive(true);    
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

    // BUTTON METHODS USED IN START GAME MENU //
    public override void OnStartGame()
    {
        if(frameCount < frames.Count() - 1)
        {
            frames[frameCount].SetActive(false);
            frameCount++;
            frames[frameCount].SetActive(true);
        }
        else
        {
            SceneManager.LoadScene("Game");
        }
    }
}
