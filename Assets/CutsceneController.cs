using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CutsceneController : MonoBehaviour
{
    private const string PlayedKey = "CutscenePlayed";

    [Header("Panels to show in order")]
    [Tooltip("Drag your 4 Sprites here in the Inspector")]
    public Sprite[] panels;

    [Header("Display Timing")]
    public float displayDuration = 2f;
    public float lastPanelFadeDuration = 1f;

    [Header("Player Setup")]
    [Tooltip("Assign your Player GameObject here")]
    public GameObject player;
    [Tooltip("If your player movement script is not on the root, assign it here")]
    public MonoBehaviour playerControllerScript;

    // internal refs
    private Image cutsceneImage;
    private Canvas thisCanvas;
    private int currentIndex = 0;

    void Awake()
    {
        // Grab UI components
        thisCanvas    = GetComponent<Canvas>();
        cutsceneImage = GetComponentInChildren<Image>();
        if (thisCanvas == null || cutsceneImage == null)
            Debug.LogError("CutsceneController must be on a Canvas with a child Image.");
    }

    void Start()
    {
        // If we've already played, just disable immediately
        if (PlayerPrefs.GetInt(PlayedKey, 0) == 1)
        {
            Destroy(gameObject);
            return;
        }

        if (panels == null || panels.Length == 0)
        {
            Debug.LogError("No panels assigned in CutsceneController!");
            Destroy(gameObject);
            return;
        }

        // Disable player movement
        if (player != null && playerControllerScript == null)
        {
            foreach (var s in player.GetComponents<MonoBehaviour>())
                s.enabled = false;
        }
        else if (playerControllerScript != null)
        {
            playerControllerScript.enabled = false;
        }

        // Ensure the image starts fully opaque
        var col = cutsceneImage.color;
        cutsceneImage.color = new Color(col.r, col.g, col.b, 1f);

        // Start the sequence
        StartCoroutine(PlayPanels());
    }

    void Update()
    {
        if (Input.anyKeyDown)
        {
            StopAllCoroutines();
            EndCutscene();
        }
    }

    IEnumerator PlayPanels()
    {
        // Show each panel at full opacity, then fade only the last
        while (currentIndex < panels.Length)
        {
            cutsceneImage.sprite = panels[currentIndex];
            cutsceneImage.color  = new Color(1f, 1f, 1f, 1f);
            yield return new WaitForSeconds(displayDuration);

            currentIndex++;

            if (currentIndex == panels.Length)
                yield return StartCoroutine(FadeLastPanel());
        }

        EndCutscene();
    }

    IEnumerator FadeLastPanel()
    {
        float elapsed = 0f;
        var   col     = cutsceneImage.color;

        while (elapsed < lastPanelFadeDuration)
        {
            float a = Mathf.Lerp(1f, 0f, elapsed / lastPanelFadeDuration);
            cutsceneImage.color = new Color(col.r, col.g, col.b, a);
            elapsed += Time.deltaTime;
            yield return null;
        }

        cutsceneImage.color = new Color(col.r, col.g, col.b, 0f);
    }

    void EndCutscene()
    {
        // Mark as played
        PlayerPrefs.SetInt(PlayedKey, 1);
        PlayerPrefs.Save();

        // Hide UI
        if (thisCanvas != null)
            thisCanvas.enabled = false;

        // Re-enable player movement
        if (player != null && playerControllerScript == null)
        {
            foreach (var s in player.GetComponents<MonoBehaviour>())
                s.enabled = true;
        }
        else if (playerControllerScript != null)
        {
            playerControllerScript.enabled = true;
        }

        Destroy(gameObject);
    }
}
