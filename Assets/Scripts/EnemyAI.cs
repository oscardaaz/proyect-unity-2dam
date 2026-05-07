using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float velocidadPatrulla = 2f;
    public float velocidadPersecucion = 4f;

    public float distanciaDeteccion = 5f;

    public float cooldownDanio = 1f;

    public float distanciaPatrulla = 3f;

    private Transform jugador;
    private Vector2 puntoInicial;
    private bool moviendoDerecha = true;
    private float tiempoUltimoDanio;
    private Rigidbody2D rb;

    void Start()
    {
        puntoInicial = transform.position;

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

        float distanciaAlJugador = Vector2.Distance(transform.position, jugador.position);

        if (distanciaAlJugador < distanciaDeteccion)
        {
            PerseguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    void Patrullar()
    {
        float direccion = moviendoDerecha ? 1f : -1f;

        rb.linearVelocity = new Vector2(direccion * velocidadPatrulla, rb.linearVelocity.y);

        transform.localScale = new Vector3(direccion, 1, 1);

        float distanciaRecorrida = transform.position.x - puntoInicial.x;

        if (distanciaRecorrida > distanciaPatrulla)
        {
            moviendoDerecha = false;
        }
        else if (distanciaRecorrida < -distanciaPatrulla)
        {
            moviendoDerecha = true;
        }
    }

    void PerseguirJugador()
    {
        float direccion = jugador.position.x > transform.position.x ? 1f : -1f;

        rb.linearVelocity = new Vector2(direccion * velocidadPersecucion, rb.linearVelocity.y);

        transform.localScale = new Vector3(direccion, 1, 1);
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
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, distanciaDeteccion);

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(
            new Vector2(transform.position.x - distanciaPatrulla, transform.position.y),
            new Vector2(transform.position.x + distanciaPatrulla, transform.position.y)
        );
    }
}