using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public enum GameState { Playing, Dead, Win }
    public GameState State { get; private set; }

    public TMP_Text textCoins;
    private int coins;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        State = GameState.Playing;
    }

    public void AddCoin()
    {
        coins++;
        textCoins.text = coins.ToString();
    }

    public void PlayerDead()
    {
        if (State != GameState.Playing) return;
        State = GameState.Dead;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void PlayerWin()
    {
        if (State != GameState.Playing) return;
        State = GameState.Win;
        SceneManager.LoadScene("WinScene");
    }
}
