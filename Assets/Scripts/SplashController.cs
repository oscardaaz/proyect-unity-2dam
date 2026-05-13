using UnityEngine;
using UnityEngine.SceneManagement;

public class SplashController : MonoBehaviour
{
    [SerializeField] private float duracion = 5f;

    [SerializeField] private string siguienteEscena = "MainMenu";

    private void Start()
    {
        Invoke(nameof(CambiarEscena), duracion);
    }

    private void CambiarEscena()
    {
        SceneManager.LoadScene(siguienteEscena);
    }
}
