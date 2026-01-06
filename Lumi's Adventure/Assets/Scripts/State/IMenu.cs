using UnityEngine;

/// <summary>
/// Define los métodos del contexto (MenuManager)
/// </summary>
public interface IMenu
{
    public GameObject GetGameObject();
    // Getters y Setters
    public void SetState(IState state);
    public IState GetState();

    // Cada estado utiliza un panel (UI)
    public GameObject GetPanel(string panel);
}
