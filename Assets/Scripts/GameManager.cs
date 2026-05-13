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
    
    private int diamonds;
    private int totalDiamonds;

    
    public TMP_Text textDiamonds;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        State = GameState.Playing;

        diamonds = 0;
        totalDiamonds = GameObject.FindObjectsByType<Diamond>(FindObjectsSortMode.None).Length;
        textDiamonds.text = diamonds + "/" + totalDiamonds;
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
        SceneManager.LoadScene("GameOver");
    }

    public void AddDiamond() { 
        diamonds++;
        textDiamonds.text = diamonds + "/" + totalDiamonds;

        if (diamonds >= totalDiamonds){
            PlayerWin();
        }
            
    }

    public void PlayerWin()
    {
        if (State != GameState.Playing) return;

        State = GameState.Win;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
