using UnityEngine;
using UnityEngine.UI; // Required if you're using Unity UI (Text, Canvas, etc.)

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue Settings")]
    [Tooltip("Lines of dialogue that the NPC will display.")]
    [SerializeField] private string[] dialogueLines;

    [Header("UI References")]
    [Tooltip("UI Text component that displays the dialogue line.")]
    [SerializeField] private Text dialogueText;
    [Tooltip("Parent panel (or canvas) that holds the dialogue UI elements.")]
    [SerializeField] private GameObject dialoguePanel;

    [Header("Player Settings")]
    [Tooltip("Reference to the player's movement script to disable/enable movement.")]
    [SerializeField] private MonoBehaviour playerMovementScript; 

    // Index of the current dialogue line
    private int currentLineIndex = 0;

    // Flag to check if the dialogue is currently active
    private bool isDialogueActive = false;

    private void Start()
    {
        // Ensure the dialogue panel is hidden at the start
        dialoguePanel.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the collider has the "Player" tag, start the dialogue
        if (other.CompareTag("Player"))
        {
            StartDialogue();
        }
    }

    private void Update()
    {
        // Only listen for input if dialogue is active
        if (isDialogueActive)
        {
            // Press 'X' to advance to the next line
            if (Input.GetKeyDown(KeyCode.X))
            {
                AdvanceDialogue();
            }
        }
    }

    /// <summary>
    /// Called when the player enters the NPC's trigger area.
    /// Disables player movement, shows the dialogue panel, and displays the first line.
    /// </summary>
    private void StartDialogue()
    {
        isDialogueActive = true;
        currentLineIndex = 0;

        // Disable the player's movement script
        if (playerMovementScript != null)
            playerMovementScript.enabled = false;

        // Show the dialogue UI
        dialoguePanel.SetActive(true);

        // Display the first line of dialogue
        if (dialogueLines.Length > 0)
        {
            dialogueText.text = dialogueLines[currentLineIndex];
        }
    }

    /// <summary>
    /// Advances to the next line of dialogue if available.
    /// If there are no more lines, ends the dialogue.
    /// </summary>
    private void AdvanceDialogue()
    {
        currentLineIndex++;
        if (currentLineIndex < dialogueLines.Length)
        {
            // Show the next line
            dialogueText.text = dialogueLines[currentLineIndex];
        }
        else
        {
            // No more lines, end the dialogue
            EndDialogue();
        }
    }

    /// <summary>
    /// Hides the dialogue UI and re-enables player movement.
    /// </summary>
    private void EndDialogue()
    {
        isDialogueActive = false;
        dialoguePanel.SetActive(false);

        // Re-enable the player's movement script
        if (playerMovementScript != null)
            playerMovementScript.enabled = true;
    }
}

