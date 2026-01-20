using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.OnPlayerEnterPortal();
        }
    }
}
