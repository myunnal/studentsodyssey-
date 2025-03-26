using UnityEngine;
using TMPro;

public class NPCDialogueTrigger : MonoBehaviour
{
    [Header("Dialogue References")]
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject dialoguePanel;

    [Header("Dialogue Content")]
    [SerializeField] private string[] dialogueLines = new string[] { 
        "Dialogas"
    };

    private int currentLineIndex = 0;
    private bool isDialogueActive = false;
    private bool hasTriggered = false;
    private PlayerController playerController;

    private void Start()
    {
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
    }

    private void Update()
    {
        if (isDialogueActive && Input.GetKeyDown(KeyCode.Space))
        {
            AdvanceDialogue();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        PlayerController playerController = other.GetComponent<PlayerController>();
        
        if (playerController != null && !hasTriggered)
        {
            StartDialogue(playerController);
            hasTriggered = true;
        }
    }

    private void StartDialogue(PlayerController player)
{
    playerController = player;

    if (playerController != null)
    {
        Animator animator = playerController.GetComponent<Animator>();
        if (animator != null)
        {
            animator.SetFloat("xVelocity", 0f);
        }

        playerController.enabled = false;
    }

    if (dialoguePanel != null)
    {
        dialoguePanel.SetActive(true);
    }

    isDialogueActive = true;

    currentLineIndex = 0;
    DisplayCurrentLine();
}

    private void DisplayCurrentLine()
    {
        if (currentLineIndex < dialogueLines.Length)
        {
            if (dialogueText != null)
            {
                dialogueText.text = dialogueLines[currentLineIndex];
            }
        }
        else
        {
            EndDialogue();
        }
    }

    private void AdvanceDialogue()
    {
        currentLineIndex++;

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
        if (dialoguePanel != null)
            dialoguePanel.SetActive(false);
        
        isDialogueActive = false;

        if (playerController != null)
        {
            playerController.enabled = true;
        }

    }
}
