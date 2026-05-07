using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    public float velocidad = 2;
    public Transform jugador;
    public float rangoDeteccion = 5;
    public int danio = 1;

    void Update()
    {
        float distancia = Vector2.Distance(transform.position, jugador.position);

        // Si el jugador está cerca -> perseguir
        if (distancia < rangoDeteccion && distancia > 1)
        {
            transform.position = Vector2.MoveTowards(
                transform.position,
                jugador.position,
                velocidad * Time.deltaTime
            );
        }
        else
        {
            // Si no tiene nada cerca -> patrullar
            transform.Translate(velocidad * Time.deltaTime, 0, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Si toca al jugador
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("El enemigo ha tocado al jugador");

            collision.gameObject.SendMessage("RecibirDanio", danio);

            // Cambiar dirección 
            velocidad = -velocidad;
        }
    }
}