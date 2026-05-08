using UnityEngine;

public class BombSpawner : MonoBehaviour
{
    [Tooltip("Prefab de la bomba que se va a generar.")]
    public GameObject bombPrefab;

    [Tooltip("Tiempo en segundos entre cada bomba generada.")]
    public float spawnInterval = 2f;

    private float nextSpawnTime;
    private GameObject currentBomb;

    void Start()
    {
        if (bombPrefab == null)
        {
            Debug.LogWarning("BombSpawner: bombPrefab no asignado.", this);
            enabled = false;
            return;
        }

        nextSpawnTime = Time.time + spawnInterval;
    }

    void Update()
    {
        if (currentBomb != null)
        {
            return;
        }

        if (Time.time >= nextSpawnTime)
        {
            currentBomb = Instantiate(bombPrefab, transform.position, Quaternion.identity);
            nextSpawnTime = Time.time + spawnInterval;
        }
    }
}
