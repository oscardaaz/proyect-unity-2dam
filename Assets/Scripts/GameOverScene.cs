using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScene : MonoBehaviour
{
    public void LoadMainMenu()
    {
        SceneTransition.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
