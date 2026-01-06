/// <summary>
/// Interfaz genérica para modelar un estado en
/// el patrón State
/// </summary>
public interface IState
{
    public void Enter();
    public void Exit();
    public void Update();
    public void FixedUpdate();
}
