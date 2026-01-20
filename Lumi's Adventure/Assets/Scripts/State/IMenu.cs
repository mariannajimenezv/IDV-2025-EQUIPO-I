using UnityEngine;

/// <summary>
/// Defines methods for context (MenuManager)
/// </summary>
public interface IMenu
{
    // Getters and Setters
    public void SetState(IState state);
    public IState GetState();

    // Each state uses a panel from MenuManager
    public GameObject GetPanel(string panel);
}
