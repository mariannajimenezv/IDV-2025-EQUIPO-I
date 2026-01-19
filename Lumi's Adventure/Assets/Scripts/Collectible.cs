using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Fragment, Heart, Exit, Sun, Moon, Star }
    public Type objectType;

    [Header("Solo para Corazones")]
    public int healAmount = 2;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LumiController lumi = other.GetComponent<LumiController>();

            if (lumi != null)
            {
                if (objectType == Type.Fragment)
                {
                    lumi.CollectFragment();
                    Destroy(gameObject);
                }
                else if (objectType == Type.Heart)
                {
                    lumi.Heal(healAmount);
                    Destroy(gameObject);
                }
                else if (objectType == Type.Exit)
                {
                    GameManager.Instance.WinLevel();
                }
                else if (objectType == Type.Sun || objectType == Type.Moon || objectType == Type.Star)
                {
                    lumi.CollectPowerUp(objectType.ToString());
                    Destroy(gameObject);
                }
            }
        }
    }
}
