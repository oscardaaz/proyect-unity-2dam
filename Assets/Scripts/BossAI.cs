using UnityEngine;

public class BossAI : MonoBehaviour
{
    public int vidaMaxima = 200;
    private int vidaActual;

    public float velocidadNormal = 3f;
    public float velocidadRapida = 6f;

    public float cooldownDanio = 0.8f;

    public float porcentajeFase2 = 0.5f;

    private Transform jugador;
    private Rigidbody2D rb;
    private float tiempoUltimoDanio;
    private bool enFase2 = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        GameObject objetoJugador = GameObject.FindGameObjectWithTag("Player");
        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (jugador == null) return;

        PerseguirJugador();
        ComprobarFase();
    }

    void PerseguirJugador()
    {
        float velocidadActual = enFase2 ? velocidadRapida : velocidadNormal;

        float direccion = jugador.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(direccion * velocidadActual, rb.linearVelocity.y);

        transform.localScale = new Vector3(direccion, 1, 1);
    }

    void ComprobarFase()
    {
        float porcentajeVida = (float)vidaActual / vidaMaxima;

        if (porcentajeVida <= porcentajeFase2 && !enFase2)
        {
            enFase2 = true;
            Debug.Log("Boss en fase 2");
        }
    }

    public void RecibirDanio(int cantidad)
    {
        vidaActual -= cantidad;

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionStay2D(Collision2D colision)
    {
        if (colision.gameObject.CompareTag("Player"))
        {
            if (Time.time >= tiempoUltimoDanio + cooldownDanio)
            {
                PlayerHealth vidaJugador = colision.gameObject.GetComponent<PlayerHealth>();

                if (vidaJugador != null)
                {
                    vidaJugador.TakeDamage();
                    tiempoUltimoDanio = Time.time;
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = enFase2 ? Color.red : Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
}
