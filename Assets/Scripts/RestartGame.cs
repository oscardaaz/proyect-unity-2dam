using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void OnRestartButton()
    {
        SceneManager.LoadScene("MainMenu");
    }
}