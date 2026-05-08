using UnityEngine;
using UnityEngine.Rendering;

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
    private SpriteRenderer[] spriteRenderers;
    private SortingGroup[] sortingGroups;
    private int explosionStateHash;
    private bool exploded;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        sortingGroups = GetComponentsInChildren<SortingGroup>();

        if (animator != null)
        {
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            animator.enabled = false;
            explosionStateHash = Animator.StringToHash("Base Layer." + explosionStateName);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (exploded)
        {
            return;
        }

        BossAI boss = collision.gameObject.GetComponent<BossAI>();

        if (boss != null)
        {
            int danioBoss = Mathf.CeilToInt(boss.vidaMaxima * 0.25f);
            boss.RecibirDanio(danioBoss);

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
            foreach (SpriteRenderer spriteRenderer in spriteRenderers)
            {
                spriteRenderer.enabled = true;
                spriteRenderer.color = Color.white;
                spriteRenderer.sortingOrder += 100;
            }

            foreach (SortingGroup sortingGroup in sortingGroups)
            {
                if (sortingGroup == null)
                {
                    continue;
                }

                sortingGroup.sortingOrder += 100;
            }

            animator.enabled = true;
            animator.Rebind();

            if (animator.HasState(0, explosionStateHash))
            {
                animator.Play(explosionStateHash, 0, 0f);
            }
            else
            {
                Debug.LogWarning("Bomb: no se encontro el estado de explosion '" + explosionStateName + "'.", this);
                animator.Play(explosionStateName, 0, 0f);
            }

            animator.Update(0f);
            Destroy(gameObject, destroyDelay);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
