using UnityEngine;

public class GoalTrigger : MonoBehaviour
{
    public string nextScene = "WinScene";

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            GameManager.Instance.PlayerWin(nextScene);
    }
}
