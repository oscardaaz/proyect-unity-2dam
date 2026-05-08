using UnityEngine;

public class Diamond : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.AddDiamond();
            Destroy(gameObject);
        }
    }
}
