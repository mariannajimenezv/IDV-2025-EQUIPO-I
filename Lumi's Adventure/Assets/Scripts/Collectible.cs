using UnityEngine;

public class Collectible : MonoBehaviour
{
    public enum Type { Fragment, Heart, Exit, Sun, Moon, Star }
    public Type objectType;

    [Header("Solo para Corazones")]
    public int healAmount = 2;
    private Vector3 startPos;

    private void Start()
    {
        startPos = transform.position;
    }

    private void Update()
    {
        // Vertical bobbing (sine wave)
        float newY = startPos.y + Mathf.Sin(Time.time * 2f) * 0.1f;
        transform.position = new Vector3(startPos.x, newY, startPos.z);

        // Constant rotation
        transform.Rotate(Vector3.up, 50f * Time.deltaTime, Space.World);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LumiController lumi = other.GetComponent<LumiController>();

            if (lumi != null)
            {
                if (objectType == Type.Fragment)
                {
                    lumi.CollectFragment(gameObject.transform);
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
