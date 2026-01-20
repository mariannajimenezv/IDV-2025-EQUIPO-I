using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] 
    private GameObject mushroomPrefab;
    [SerializeField]
    private GameObject cactusPrefab;

    int enemyType;


    private void Awake()
    {
        enemyType = Random.Range(0, 2); // 0: Mushroom, 1: Cactus

        if (enemyType == 0)
        {
            Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
            Debug.Log("Seta creada");

        }
        else
        {
            Instantiate(cactusPrefab, transform.position, Quaternion.identity);
            Debug.Log("Cactus creado");

        }
    }

}
