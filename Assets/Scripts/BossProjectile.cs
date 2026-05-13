using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    public float tiempoDeVida = 5f;

    private bool haGolpeado;

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        IntentarGolpearJugador(collision.gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        IntentarGolpearJugador(collision.gameObject);
    }

    void IntentarGolpearJugador(GameObject objetivo)
    {
        if (haGolpeado)
        {
            return;
        }

        PlayerHealth vidaJugador = objetivo.GetComponentInParent<PlayerHealth>();

        if (vidaJugador == null || !vidaJugador.CompareTag("Player"))
        {
            return;
        }

        vidaJugador.TakeDamage();

        haGolpeado = true;
        Destroy(gameObject);
    }
}
