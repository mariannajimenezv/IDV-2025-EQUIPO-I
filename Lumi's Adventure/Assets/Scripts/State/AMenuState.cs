/// <summary>
/// Template used for menu screens using State pattern
/// based on IState interface
/// </summary>
public abstract class AMenuState : IState
{
    protected IMenu menu;

    // Constructor
    public AMenuState(IMenu menu) => this.menu = menu;

    // State properties and transitions
    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();

    // BUTTON METHODS USED IN MENUS //

    // Main Menu
    public virtual void OnTutorial() {}
    public virtual void OnPlay(){}
    public virtual void OnSettings() {}
    public virtual void OnCredits() {}

    // Start Game Screen
    public virtual void OnStartGame() {}

    // Generic use 
    public virtual void OnMainMenu() {}
    public virtual void OnBack() {}
    
}
