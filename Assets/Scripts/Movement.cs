using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float horizontalInput;
    public float moveSpeed = 5f;
    public float jumpPower = 10f;
    public bool isFacingRight = false;
    private bool isGrounded = false;
    private Rigidbody2D rb;
    private Animator animator;

    // Idle-timer fields
    public float idleDelay = 1f;
    private float timeSinceMove = 0f;
    private bool isMoving = false;


    [Header("Audio")]
    public AudioSource audioSource;    
    public AudioClip jumpClip;         // The jump sound
    public AudioClip walkClip;         // The walking/footstep loop


    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        horizontalInput = Input.GetAxis("Horizontal");

        FlipSprite();

        // JUMP
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpPower);
            isGrounded = false;
            animator.SetBool("isJumping", true);

            // Play jump sound once
            audioSource.PlayOneShot(jumpClip);
        }

        // Check horizontal movement
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            timeSinceMove = 0f;
            isMoving = true;

            // If not already playing the walk sound, loop it
            if (!audioSource.isPlaying && walkClip != null && isGrounded)
            {
                audioSource.clip = walkClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }
        else
        {
            // Not moving horizontally
            timeSinceMove += Time.deltaTime;
            isMoving = false;

            // Stop walk sound if playing
            if (audioSource.isPlaying && audioSource.clip == walkClip)
            {
                audioSource.Stop();
            }
        }

        // Idle logic
        if (!isMoving && timeSinceMove >= idleDelay)
        {
            animator.SetBool("isIdle", true);
        }
        else
        {
            animator.SetBool("isIdle", false);
        }
    }

    private void FixedUpdate()
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
}
