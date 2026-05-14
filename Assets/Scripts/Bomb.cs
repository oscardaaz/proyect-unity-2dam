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

    [Tooltip("Orden de renderizado normal de la bomba.")]
    public int sortingOrder = 10;

    [Tooltip("Orden extra que se suma al explotar para que la animacion se vea por encima.")]
    public int explosionSortingBoost = 100;

    [Tooltip("Radio en el que la explosion puede danar al Boss.")]
    public float bossDamageRadius = 2f;

    private Animator animator;
    private Rigidbody2D rb;
    private Collider2D[] colliders;
    private SpriteRenderer[] spriteRenderers;
    private SortingGroup[] sortingGroups;
    private int explosionStateHash;
    private bool exploded;
    private bool bossDamaged;

    void Awake()
    {
        animator = GetComponentInChildren<Animator>(true);
        rb = GetComponent<Rigidbody2D>();
        colliders = GetComponents<Collider2D>();
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>(true);
        sortingGroups = GetComponentsInChildren<SortingGroup>(true);

        if (animator != null)
        {
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
            explosionStateHash = Animator.StringToHash("Base Layer." + explosionStateName);
        }

        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.gameObject.SetActive(true);
            spriteRenderer.enabled = true;
            spriteRenderer.color = Color.white;
            spriteRenderer.sortingOrder = sortingOrder;
        }

        foreach (SortingGroup sortingGroup in sortingGroups)
        {
            if (sortingGroup == null)
            {
                continue;
            }

            sortingGroup.sortingOrder = sortingOrder;
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
            DamageBoss(boss);

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

        DamageNearbyBoss();

        PlayExplosionSound();

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
                spriteRenderer.sortingOrder = sortingOrder + explosionSortingBoost;
            }

            foreach (SortingGroup sortingGroup in sortingGroups)
            {
                if (sortingGroup == null)
                {
                    continue;
                }

                sortingGroup.sortingOrder = sortingOrder + explosionSortingBoost;
            }

            animator.enabled = true;

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

    void PlayExplosionSound()
    {
        if (explosionSound == null)
        {
            return;
        }

        GameObject soundObject = new GameObject("BombExplosionSound");
        AudioSource audioSource = soundObject.AddComponent<AudioSource>();
        audioSource.clip = explosionSound;
        audioSource.volume = explosionVolume;
        audioSource.spatialBlend = 0f;
        audioSource.Play();

        Destroy(soundObject, explosionSound.length);
    }

    void DamageNearbyBoss()
    {
        if (bossDamaged)
        {
            return;
        }

        BossAI[] bosses = FindObjectsByType<BossAI>(FindObjectsSortMode.None);

        foreach (BossAI boss in bosses)
        {
            if (Vector2.Distance(transform.position, boss.transform.position) <= bossDamageRadius)
            {
                DamageBoss(boss);
                return;
            }
        }
    }

    void DamageBoss(BossAI boss)
    {
        if (boss == null || bossDamaged)
        {
            return;
        }

        int danioBoss = boss.ObtenerDanioBomba();
        boss.RecibirDanio(danioBoss, true);
        bossDamaged = true;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, bossDamageRadius);
    }
}
