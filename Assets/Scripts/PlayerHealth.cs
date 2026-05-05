using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxLives = 3;
    private int lives;

    public float iFramesDuration = 1.5f;
    private bool isInvincible;

    private Animator animator;

    void Start()
    {
        lives = maxLives;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage()
    {
        if (isInvincible) return;
        lives--;
        if (lives <= 0)
        {
            GameManager.Instance.PlayerDead();
            return;
        }
        StartCoroutine(IFrames());
    }

    IEnumerator IFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iFramesDuration);
        isInvincible = false;
    }
}
