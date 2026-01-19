using UnityEngine;

public class ScoreManager : MonoBehaviour, IScoreService
{
    public int CurrentScore { get; private set; }

    private void Awake()
    {
        ServiceLocator.Register<IScoreService>(this);
    }

    public void AddPoints()
    {
        CurrentScore++;
    }
}
