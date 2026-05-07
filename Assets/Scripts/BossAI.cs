using UnityEngine;

public class BossAI : MonoBehaviour
{
    public float velocidad = 2;
    public Transform jugador;
    public int vida = 10;
    public float rangoDeteccion = 6;
    public int danio = 2;

    void Update()
    {
        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Mayor vida jefe final
        if (vida > 5)
        {
            if (distancia < rangoDeteccion)
            {
                transform.position = Vector2.MoveTowards(
                    transform.position,
                    jugador.position,
                    velocidad * Time.deltaTime
                );
            }
        }
        else
        {
            // FASE 2 (vida baja -> más agresivo)
            transform.position = Vector2.MoveTowards(
                transform.position,
                jugador.position,
                velocidad * 2 * Time.deltaTime
            );
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Boss golpea al jugador");

            collision.gameObject.SendMessage("RecibirDanio", danio);
        }
    }

    // Método para que el boss reciba daño
    public void RecibirDanio(int cantidad)
    {
        vida -= cantidad;
        Debug.Log("Vida Boss: " + vida);

        if (vida <= 0)
        {
            Debug.Log("Boss derrotado");
            Destroy(gameObject);
        }
    }
}