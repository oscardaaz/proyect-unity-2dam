using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float velocidad = 2;

    void Update()
    {
        transform.Translate(velocidad * Time.deltaTime, 0, 0);
    }
}