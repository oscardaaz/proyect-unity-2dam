using UnityEngine;

public class Player : MonoBehaviour
{
    public float speed = 5;
    public float jumpForce = 4;
    public Transform groundCheck;
    public float groundRadius = 0.1f;
    public LayerMask groundLayer;

    private Rigidbody2D rb2D;
    private Animator animator;
    private PlayerHealth health;
    private float move;
    private bool isGrounded;

    void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        health = GetComponent<PlayerHealth>();
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
    }

    void FixedUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin"))
        {
            Destroy(collision.gameObject);
            GameManager.Instance.AddCoin();
        }

        if (collision.CompareTag("Spikes"))
            health.TakeDamage();

        if (collision.CompareTag("Barrel"))
        {
            Vector2 knockbackDir = (rb2D.position - (Vector2)collision.transform.position).normalized;
            rb2D.linearVelocity = Vector2.zero;
            rb2D.AddForce(knockbackDir * 3, ForceMode2D.Impulse);

            foreach (BoxCollider2D col in collision.gameObject.GetComponents<BoxCollider2D>())
                col.enabled = false;

            collision.GetComponent<Animator>().enabled = true;
            Destroy(collision.gameObject, 0.5f);

            health.TakeDamage();
        }
    }
}
