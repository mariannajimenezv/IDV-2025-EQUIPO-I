using UnityEngine;

public interface IFlyweightEnemy
{
    public void Awake();
    public void Update();

    public void IdlePatrol();
    public void SetPatrolPoint();

    public void ChasingPlayer();

    public void Attack();
    public void ResetAttack();

    //public void OnCollisionEnter(Collision collision);

    public void Die();
}
