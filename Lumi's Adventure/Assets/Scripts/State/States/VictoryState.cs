using UnityEngine;
using UnityEngine.SceneManagement;

public class VictoryState : AMenuState
{
    private readonly GameObject panel;

    public VictoryState(IMenu menu): base(menu)
    {
        this.menu = menu; 
        panel = menu.GetPanel("VictoryState");
    }

    public override void Enter()
    {
        if (panel != null) panel.SetActive(true);
        Time.timeScale = 0; 

        // Opcional
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public override void Exit()
    {
        if (panel != null) panel.SetActive(false);
    }

    public override void Update()
    {

    }

    public override void FixedUpdate()
    {
        
    }

    public override void OnMainMenu()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene("Menu");
    }
}