using UnityEngine;

public class Diamond : MonoBehaviour
{
    public AudioClip diamondClip;
    
    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            AudioSource playerAudio = collision.GetComponent<AudioSource>();

            if (playerAudio != null && diamondClip != null)
            {
                playerAudio.PlayOneShot(diamondClip);
            }
            GameManager.Instance.AddDiamond();
            Destroy(gameObject);
        }
    }
}
