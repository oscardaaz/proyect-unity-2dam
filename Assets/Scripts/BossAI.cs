using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossAI : MonoBehaviour
{
    public int vidaMaxima = 250;
    private int vidaActual;

    public float velocidadNormal = 2f;
    public float velocidadRapida = 4f;

    public float cooldownDanio = 0.8f;

    public float porcentajeFase2 = 0.5f;
    public int golpesParaMorir = 5;
    public string bossIconObjectName = "BossIcon";
    public string bossLifeObjectNamePrefix = "BossLife";

    public Vector2 puntoIzquierdo = new Vector2(-13.7f, 4f);
    public Vector2 puntoDerecho = new Vector2(-6.2f, 4f);

    public GameObject proyectilAtaquePrefab;
    public Transform puntoLanzamiento;
    public Vector2 desplazamientoLanzamiento = new Vector2(0f, 0.65f);
    public Vector2 velocidadProyectil = new Vector2(0f, 9f);
    public int framesMinimosEntreAtaques = 30;
    public int framesMaximosEntreAtaques = 90;
    public float duracionAnimacionAtaque = 0.7f;
    public float retardoLanzamiento = 0.3f;
    public string estadoCaminar = "Walk";
    public string estadoAtaque = "Attack";

    private Rigidbody2D rb;
    private Animator animator;
    private float tiempoUltimoDanio;
    private bool enFase2 = false;
    private Vector2 objetivoPatrulla;
    private Vector3 escalaInicial;
    private int framesHastaAtaque;
    private bool atacando;
    private bool bossDerrotado;
    private GameObject bossIconHUD;
    private GameObject[] bossLivesHUD;

    void Start()
    {
        vidaActual = vidaMaxima;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        escalaInicial = transform.localScale;
        objetivoPatrulla = puntoIzquierdo;
        BuscarHUDVidaBoss();
        ActualizarHUDVidaBoss();
        ProgramarSiguienteAtaque();
    }

    void Update()
    {
        ComprobarFase();
        ComprobarAtaque();
    }

    void FixedUpdate()
    {
        PatrullarPlataforma();
    }

    void PatrullarPlataforma()
    {
        float velocidadActual = enFase2 ? velocidadRapida : velocidadNormal;
        Vector2 posicionActual = rb.position;

        if (Vector2.Distance(posicionActual, objetivoPatrulla) < 0.05f)
        {
            objetivoPatrulla = objetivoPatrulla == puntoIzquierdo ? puntoDerecho : puntoIzquierdo;
        }

        Vector2 nuevaPosicion = Vector2.MoveTowards(posicionActual, objetivoPatrulla, velocidadActual * Time.fixedDeltaTime);
        float direccion = objetivoPatrulla.x > posicionActual.x ? 1f : -1f;

        rb.MovePosition(nuevaPosicion);

        transform.localScale = new Vector3(Mathf.Abs(escalaInicial.x) * -direccion, escalaInicial.y, escalaInicial.z);
    }

    void ComprobarAtaque()
    {
        if (atacando)
        {
            return;
        }

        framesHastaAtaque--;

        if (framesHastaAtaque <= 0)
        {
            StartCoroutine(Atacar());
        }
    }

    IEnumerator Atacar()
    {
        atacando = true;

        if (animator != null)
        {
            animator.Play(estadoAtaque, 0, 0f);
        }

        yield return new WaitForSeconds(retardoLanzamiento);

        LanzarProyectil();

        yield return new WaitForSeconds(Mathf.Max(0f, duracionAnimacionAtaque - retardoLanzamiento));

        if (animator != null)
        {
            animator.Play(estadoCaminar, 0, 0f);
        }

        atacando = false;
        ProgramarSiguienteAtaque();
    }

    void LanzarProyectil()
    {
        if (proyectilAtaquePrefab == null)
        {
            return;
        }

        Vector3 posicionLanzamiento = puntoLanzamiento != null
            ? puntoLanzamiento.position
            : transform.position + new Vector3(desplazamientoLanzamiento.x * Mathf.Sign(transform.localScale.x), desplazamientoLanzamiento.y, 0f);

        GameObject proyectil = Instantiate(proyectilAtaquePrefab, posicionLanzamiento, Quaternion.identity);
        Rigidbody2D rbProyectil = proyectil.GetComponent<Rigidbody2D>();

        if (rbProyectil != null)
        {
            rbProyectil.linearVelocity = velocidadProyectil;
        }
    }

    void ProgramarSiguienteAtaque()
    {
        int minimo = Mathf.Max(1, framesMinimosEntreAtaques);
        int maximo = Mathf.Max(minimo, framesMaximosEntreAtaques);
        framesHastaAtaque = Random.Range(minimo, maximo + 1);
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

    public void RecibirDanio(int cantidad, bool causadoPorBomba = false)
    {
        if (!causadoPorBomba)
        {
            return;
        }

        vidaActual -= cantidad;
        ActualizarHUDVidaBoss();

        if (vidaActual <= 0 && !bossDerrotado)
        {
            bossDerrotado = true;
            SceneManager.LoadScene("WinScene");
            Destroy(gameObject);
        }
    }

    public int ObtenerDanioBomba()
    {
        return Mathf.CeilToInt((float)vidaMaxima / Mathf.Max(1, golpesParaMorir));
    }

    void ActualizarHUDVidaBoss()
    {
        if (bossLivesHUD == null)
        {
            return;
        }

        int danioPorGolpe = ObtenerDanioBomba();
        int vidasRestantes = Mathf.CeilToInt(Mathf.Max(0, vidaActual) / (float)danioPorGolpe);

        for (int i = 0; i < bossLivesHUD.Length; i++)
        {
            if (bossLivesHUD[i] != null)
            {
                bossLivesHUD[i].SetActive(i < vidasRestantes);
            }
        }
    }

    void BuscarHUDVidaBoss()
    {
        bossIconHUD = BuscarObjetoEnEscena(bossIconObjectName);

        if (bossIconHUD != null)
        {
            bossIconHUD.SetActive(true);
        }

        bossLivesHUD = new GameObject[Mathf.Max(1, golpesParaMorir)];

        for (int i = 0; i < bossLivesHUD.Length; i++)
        {
            bossLivesHUD[i] = BuscarObjetoEnEscena(bossLifeObjectNamePrefix + (i + 1));
        }
    }

    GameObject BuscarObjetoEnEscena(string objectName)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (Transform sceneObject in transforms)
        {
            if (sceneObject.name == objectName && sceneObject.gameObject.scene.IsValid())
            {
                return sceneObject.gameObject;
            }
        }

        return null;
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

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(puntoIzquierdo, puntoDerecho);
    }
}
