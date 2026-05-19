using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScene : MonoBehaviour
{
    private void Start()
    {
        MenuVolumeControl.CreateBelowButtons(
            this,
            new[] { "ButtonRestart", "ButtonQuit" },
            -118f,
            new Vector2(430f, 82f),
            42f,
            42f,
            new Vector2(410f, 30f),
            new Vector2(30f, 30f));
    }

    public void LoadMainMenu()
    {
        SceneTransition.LoadScene("MainMenu");
    }
    public void QuitGame()
    {
        Application.Quit();
    }
}

