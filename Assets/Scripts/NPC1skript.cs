using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue References")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Dialogue Content")]
    [SerializeField]
    private string[] dialogueLines = new string[]
    {
        "Dialogas"
    };

    [Header("Audio")]
    [SerializeField] private AudioSource speakingSound; // [EDITED] Added AudioSource for speaking sound

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool hasTriggered = false;
    private PlayerController playerController;

    private void Start()
    {
        // Ensure dialogue panel is hidden at start
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        // Check for advancing dialogue when Space is pressed and dialogue is active
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Try to find the PlayerController script on the colliding object
        PlayerController playerController = other.GetComponent<PlayerController>();

        if (playerController != null && !hasTriggered)
        {
            // Start dialogue and mark as triggered
            StartDialogue(playerController);
            hasTriggered = true;  // Dialogue won't trigger again after this
        }
    }

    private void StartDialogue(PlayerController player)
    {
        // Store reference to player controller script
        playerController = player;

        // Disable player movement
        if (playerController != null)
        {
            // Force the blend tree to "Stand" by setting the parameter to 0
            Animator animator = playerController.GetComponent<Animator>();
            if (animator != null)
            {
                animator.SetFloat("xVelocity", 0f);
            }

            // Disable the PlayerController script to prevent movement
            playerController.enabled = false;
        }

        // Activate dialogue panel
        if (dialoguePanel != null)
        {
            dialoguePanel.SetActive(true);
        }

        isDialogueActive = true;

        // Reset to first line
        currentLineIndex = 0;
        DisplayCurrentLine();
    }

    private void DisplayCurrentLine()
    {
        // Display current dialogue line
        if (currentLineIndex < dialogueLines.Length)
        {
            if (dialogueText != null)
            {
                dialogueText.text = dialogueLines[currentLineIndex];
            }

            // [EDITED] Moved playing sound inside the correct condition
            if (speakingSound != null)
            {
                speakingSound.pitch = Random.Range(0.95f, 1.05f);
                speakingSound.Play();
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        // Move to next line
        currentLineIndex++;

        // Check if more lines exist
        if (currentLineIndex < dialogueLines.Length)
        {
            DisplayCurrentLine();
        }
        else
        {
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        // Hide dialogue panel
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);

        isDialogueActive = false;

        // Re-enable player movement
        if (playerController != null)
        {
            playerController.enabled = true;
        }

        // hasTriggered = false;
    }
}