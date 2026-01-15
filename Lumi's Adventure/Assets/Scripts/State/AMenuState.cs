using UnityEngine;

public abstract class AMenuState : IState
{
    protected IMenu menu;
    public AMenuState(IMenu menu) => this.menu = menu;

    public abstract void Enter();
    public abstract void Exit();
    public abstract void Update();
    public abstract void FixedUpdate();

    public virtual void OnPlay() {}
    public virtual void OnSettings() {}
    public virtual void OnCredits() {}
    public virtual void OnMainMenu() {}
    public virtual void OnBack() {}
    
}
