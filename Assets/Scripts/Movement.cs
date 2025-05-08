using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float moveSpeed = 5f;
    public float jumpPower = 10f;
    public bool isFacingRight = false;
    private bool isGrounded = false;
    private Rigidbody2D rb;
    private Animator animator;

    // Dialogue lock
    public bool inDialogue = false;

    // Idle-timer fields
    public float idleDelay = 1f;
    private float timeSinceMove = 0f;
    private bool isMoving = false;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip jumpClip;
    public AudioClip walkClip;
    
    [Header("Spawn Settings")]
    public Transform spawnPoint;

    // Static variables to store the last known position
    private static Vector3 lastPosition = Vector3.zero;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (inDialogue)
        {
            horizontalInput = 0f;

            if (audioSource.isPlaying && audioSource.clip == walkClip)
            {
                audioSource.Stop(); // Stop walking sound
            }

            animator.SetBool("isIdle", true); // Ensure idle animation plays
            return;
        }

        horizontalInput = Input.GetAxis("Horizontal");
        FlipSprite();

        // JUMP
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", true);
            audioSource.PlayOneShot(jumpClip);
        }

        // Check horizontal movement
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            timeSinceMove = 0f;
            isMoving = true;
            if (!audioSource.isPlaying && walkClip != null && isGrounded)
            {
                audioSource.clip = walkClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            timeSinceMove += Time.deltaTime;
            isMoving = false;
            if (audioSource.isPlaying && audioSource.clip == walkClip)
            {
                audioSource.Stop();
            }
        }

        // Idle logic
        animator.SetBool("isIdle", !isMoving && timeSinceMove >= idleDelay);
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(horizontalInput * moveSpeed, rb.velocity.y);
        animator.SetFloat("xVelocity", Mathf.Abs(rb.velocity.x));
        animator.SetFloat("yVelocity", rb.velocity.y);
    }

    void FlipSprite()
    {
        if ((isFacingRight && horizontalInput < 0f) ||
            (!isFacingRight && horizontalInput > 0f))
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!isGrounded)
        {
            isGrounded = true;
            animator.SetBool("isJumping", false);
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "LevelPicker" || scene.name == "Meniu")
        {
            Destroy(gameObject);  // Destroy player when in non-gameplay scenes
            return;
        }

        if (spawnPoint != null)
        {
            transform.position = spawnPoint.position;  // Move player to spawn point
        }

        if (transform.position.y < 0)  // If Y position is below ground
        {
            transform.position = new Vector3(transform.position.x, 0, transform.position.z); // Set to Y=0
            Debug.Log("Player position adjusted to ground level.");
        }
    }
}
