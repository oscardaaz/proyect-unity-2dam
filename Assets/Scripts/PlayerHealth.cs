using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    private int lives;

    public float iFramesDuration = 1.5f;
    private bool isInvincible;

    public GameObject[] hearts;

    private Animator animator;

    void Start()
    {
        lives = maxLives;
        animator = GetComponent<Animator>();
        UpdateHeartsUI();
    }

    public void TakeDamage()
    {
        if (isInvincible) return;
        lives--;
        UpdateHeartsUI();
        if (lives <= 0)
        {
            GameManager.Instance.PlayerDead();
            return;
        }
        StartCoroutine(IFrames());
    }

    void UpdateHeartsUI()
    {
        for (int i = 0; i < hearts.Length; i++)
            hearts[i].SetActive(i < lives);
    }

    IEnumerator IFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iFramesDuration);
        isInvincible = false;
    }
}
