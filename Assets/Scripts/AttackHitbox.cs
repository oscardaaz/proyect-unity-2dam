using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public int damage = 1;

    void OnTriggerEnter2D(Collider2D collision)
    {
        BossAI boss = collision.GetComponent<BossAI>();

        if (boss != null)
        {
            boss.RecibirDanio(damage);
        }
    }
}
