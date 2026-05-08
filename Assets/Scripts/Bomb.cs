using UnityEngine;

public class Bomb : MonoBehaviour
{
    [Tooltip("Tag del trigger invisible que hace explotar la bomba.")]
    public string explosionZoneTag = "BombExplosionZone";

    [Tooltip("Nombre exacto del estado de explosion en el Animator.")]
    public string explosionStateName = "Explosion";

    [Tooltip("Tiempo que tarda en destruirse despues de empezar la explosion.")]
    public float destroyDelay = 0.4f;

    [Tooltip("Sonido que se reproduce al explotar.")]
    public AudioClip explosionSound;

    [Range(0f, 1f)]
    public float explosionVolume = 1f;

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private bool exploded;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();

        if (animator != null)
        {
            animator.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded)
        {
            return;
        }

        if (collision.gameObject.CompareTag("Boss"))
        {
            BossAI boss = collision.gameObject.GetComponent<BossAI>();

            if (boss != null)
            {
                int danioBoss = Mathf.CeilToInt(boss.vidaMaxima * 0.25f);
                boss.RecibirDanio(danioBoss);
            }

            Explode();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (exploded)
        {
            return;
        }

        if (collision.CompareTag(explosionZoneTag))
        {
            Explode();
        }
    }

    void Explode()
    {
        exploded = true;
        Debug.Log("BOOM");

        if (explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(explosionSound, transform.position, explosionVolume);
        }

        foreach (Collider2D bombCollider in colliders)
        {
            bombCollider.enabled = false;
        }

        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        transform.rotation = Quaternion.identity;

        if (animator != null)
        {
            animator.enabled = true;
            animator.Play(explosionStateName, 0, 0f);
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
