using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 4;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;
    public GameObject attackHitbox;
    public float attackCooldown = 0.5f;

    private Rigidbody2D rb2D;
    private Animator animator;
    private PlayerHealth health;
    private float move;
    private bool isGrounded;
    private bool canAttack = true;

    public AudioSource audioSource;
    public AudioClip coinClip;
    public AudioClip barrelClip;
    public AudioClip spikeClip;
    public AudioClip diamondClip;

    private const string FirstMapSceneName = "mapa_1";
    private const string BossMapSceneName = "mapa_Boss";
    private const string FirstMapTutorialMessage = "A y D para caminar. Espacio para saltar";
    private const string BossMapTutorialMessage = "Empuja las bombas para vencer al minotauro";
    private const float TutorialVisibleTime = 5f;
    private const float TutorialFadeTime = 1f;
    private static readonly Vector3 TutorialWorldOffset = new Vector3(0f, 1.1f, 0f);


    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();

        string activeSceneName = SceneManager.GetActiveScene().name;

        if (activeSceneName == FirstMapSceneName)
            ShowTutorialBubble(FirstMapTutorialMessage);
        else if (activeSceneName == BossMapSceneName)
            ShowTutorialBubble(BossMapTutorialMessage);
        else
            Debug.Log("No se muestra el bocadillo tutorial porque la escena actual es: " + activeSceneName);
    }

    private void ShowTutorialBubble(string tutorialMessage)
    {
        Debug.Log("Creando bocadillo tutorial sobre el jugador");

        Camera mainCamera = Camera.main;

        if (mainCamera == null)
            mainCamera = FindFirstObjectByType<Camera>();

        if (mainCamera == null)
        {
            Debug.LogWarning("No se encontro ninguna camara para colocar el bocadillo tutorial.");
            return;
        }

        GameObject canvasObject = new GameObject("TutorialBubbleCanvas");
        Canvas canvas = canvasObject.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 1000;
        canvasObject.AddComponent<CanvasScaler>();
        canvasObject.AddComponent<GraphicRaycaster>();

        GameObject bubbleObject = new GameObject("TutorialBubble");
        bubbleObject.transform.SetParent(canvasObject.transform, false);

        RectTransform bubbleRect = bubbleObject.AddComponent<RectTransform>();
        bubbleRect.sizeDelta = new Vector2(560f, 90f);

        CanvasGroup bubbleGroup = bubbleObject.AddComponent<CanvasGroup>();

        GameObject backgroundObject = new GameObject("TutorialBubbleBackground");
        backgroundObject.transform.SetParent(bubbleObject.transform, false);

        RectTransform backgroundRect = backgroundObject.AddComponent<RectTransform>();
        backgroundRect.anchorMin = Vector2.zero;
        backgroundRect.anchorMax = Vector2.one;
        backgroundRect.offsetMin = Vector2.zero;
        backgroundRect.offsetMax = Vector2.zero;

        Image bubbleBackground = backgroundObject.AddComponent<Image>();
        bubbleBackground.color = Color.white;

        GameObject tailObject = new GameObject("TutorialBubbleTail");
        tailObject.transform.SetParent(bubbleObject.transform, false);

        RectTransform tailRect = tailObject.AddComponent<RectTransform>();
        tailRect.anchorMin = new Vector2(0.5f, 0f);
        tailRect.anchorMax = new Vector2(0.5f, 0f);
        tailRect.pivot = new Vector2(0.5f, 0.5f);
        tailRect.anchoredPosition = new Vector2(0f, -24f);
        tailRect.sizeDelta = new Vector2(34f, 34f);
        tailRect.localRotation = Quaternion.Euler(0f, 0f, 45f);

        Image tailBackground = tailObject.AddComponent<Image>();
        tailBackground.color = Color.white;

        GameObject textObject = new GameObject("TutorialText");
        textObject.transform.SetParent(bubbleObject.transform, false);

        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = new Vector2(18f, 10f);
        textRect.offsetMax = new Vector2(-18f, -10f);

        TextMeshProUGUI tutorialText = textObject.AddComponent<TextMeshProUGUI>();
        tutorialText.text = tutorialMessage;
        tutorialText.fontSize = 28f;
        tutorialText.fontStyle = FontStyles.Bold;
        tutorialText.alignment = TextAlignmentOptions.Center;
        tutorialText.color = Color.black;
        tutorialText.enableWordWrapping = true;

        StartCoroutine(FadeTutorialBubble(canvasObject, bubbleRect, bubbleGroup, mainCamera));
    }

    private IEnumerator FadeTutorialBubble(GameObject canvasObject, RectTransform bubbleRect, CanvasGroup bubbleGroup, Camera mainCamera)
    {
        float elapsed = 0f;

        while (elapsed < TutorialVisibleTime)
        {
            elapsed += Time.deltaTime;
            bubbleRect.position = mainCamera.WorldToScreenPoint(transform.position + TutorialWorldOffset);
            yield return null;
        }

        elapsed = 0f;

        while (elapsed < TutorialFadeTime)
        {
            elapsed += Time.deltaTime;
            bubbleGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / TutorialFadeTime);
            bubbleRect.position = mainCamera.WorldToScreenPoint(transform.position + TutorialWorldOffset);

            yield return null;
        }

        Destroy(canvasObject);
    }

    void Update()
    {
        move = Input.GetAxisRaw("Horizontal");
        rb2D.linearVelocity = new Vector2(move * speed, rb2D.linearVelocity.y);

        if (move != 0)
            transform.localScale = new Vector3(Mathf.Sign(move), 1, 1);

        if (Input.GetButtonDown("Jump") && isGrounded)
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpForce);

        animator.SetFloat("Speed", Mathf.Abs(move));
        animator.SetFloat("VerticalVelocity", rb2D.linearVelocity.y);
        animator.SetBool("isGrounded", isGrounded);

        if (Input.GetButtonDown("Fire1") && canAttack)
            StartCoroutine(Attack());
    }

    private System.Collections.IEnumerator Attack()
    {
        canAttack = false;
        animator.SetTrigger("Attack");
        attackHitbox.SetActive(true);
        yield return new WaitForSeconds(0.2f);
        attackHitbox.SetActive(false);
        yield return new WaitForSeconds(attackCooldown - 0.2f);
        canAttack = true;
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Coin"))
        {
            audioSource.PlayOneShot(coinClip);
            Destroy(collision.gameObject);
            GameManager.Instance.AddCoin();
        }

        if (collision.CompareTag("Spikes"))
        {
            audioSource.PlayOneShot(spikeClip);
            health.TakeDamage();
        }

        if (collision.CompareTag("Barrel"))
        {
            audioSource.PlayOneShot(barrelClip);
            Vector2 knockbackDir = (rb2D.position - (Vector2)collision.transform.position).normalized;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            foreach (BoxCollider2D col in collision.gameObject.GetComponents<BoxCollider2D>())
                col.enabled = false;

            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);

        }
    }
}
