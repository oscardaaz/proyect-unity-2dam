using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScene : MonoBehaviour 
{
    void Start()
    {
        Invoke("LoadMainMenu",3);
    }

    public void LoadMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}

