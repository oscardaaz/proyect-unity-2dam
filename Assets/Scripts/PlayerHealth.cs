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
        FindHeartsIfNeeded();
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
            if (hearts[i] != null)
                hearts[i].SetActive(i < lives);
    }

    void FindHeartsIfNeeded()
    {
        if (hearts != null && hearts.Length >= maxLives)
        {
            bool hasAllHearts = true;

            for (int i = 0; i < maxLives; i++)
            {
                if (hearts[i] == null)
                {
                    hasAllHearts = false;
                    break;
                }
            }

            if (hasAllHearts)
            {
                return;
            }
        }

        hearts = new GameObject[maxLives];

        for (int i = 0; i < hearts.Length; i++)
        {
            hearts[i] = FindSceneObject("Heart" + (i + 1));
        }
    }

    GameObject FindSceneObject(string objectName)
    {
        Transform[] transforms = Resources.FindObjectsOfTypeAll<Transform>();

        foreach (Transform sceneObject in transforms)
        {
            if (sceneObject.name == objectName && sceneObject.gameObject.scene.IsValid())
            {
                return sceneObject.gameObject;
            }
        }

        return null;
    }

    IEnumerator IFrames()
    {
        isInvincible = true;
        yield return new WaitForSeconds(iFramesDuration);
        isInvincible = false;
    }
}
