using UnityEngine;
using UnityEngine.SceneManagement;

public class RestartGame : MonoBehaviour
{
    public void OnRestartButton()
    {
        SceneTransition.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}
