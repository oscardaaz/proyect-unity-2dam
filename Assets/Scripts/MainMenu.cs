using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        MenuVolumeControl.CreateBelowButtons(this, "ButtonPlay", "ButtonQuit");
    }

    public void Play()
    {
        SceneTransition.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Quit()
    {
        Application.Quit();
    }

}
