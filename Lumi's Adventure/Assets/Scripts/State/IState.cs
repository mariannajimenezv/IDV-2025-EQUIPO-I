/// <summary>
/// Generic interface for modelling a state in State pattern
/// </summary>
public interface IState
{
    public void Enter();
    public void Exit();
    public void Update();
    public void FixedUpdate();
}
