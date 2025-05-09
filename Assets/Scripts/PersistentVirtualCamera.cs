using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PersistentVirtualCamera : MonoBehaviour
{
    private static PersistentVirtualCamera instance;
    private CinemachineVirtualCamera virtualCamera;
    private CinemachineConfiner confiner;
    private Transform playerTransform;
    
    private static Vector3 lastPlayerPosition;
    private static bool hasStoredPosition = false;
    private static string previousScene = "";
    private bool isInMenuScene = false;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            confiner = GetComponent<CinemachineConfiner>();

            if (virtualCamera == null) Debug.LogError("Missing CinemachineVirtualCamera!");
            if (confiner == null) Debug.LogWarning("Missing CinemachineConfiner - camera bounds disabled.");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start() => FindPlayer();

    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;
        isInMenuScene = currentSceneName == "LevelPicker" || 
                        currentSceneName == "Meniu" || 
                        currentSceneName == "CollectablesMenu";

        if (isInMenuScene)
        {
            StorePlayerPosition();
            gameObject.SetActive(false);
        }
        else
        {
            gameObject.SetActive(true);
            HandleGameplaySceneEntry();
        }

        previousScene = currentSceneName;
    }

    void HandleGameplaySceneEntry()
    {
        FindPlayer();
        UpdateCameraBounds();
        RestorePlayerPosition();
    }

    void UpdateCameraBounds()
    {
        if (confiner == null) return;

        // Clear previous bounds reference
        confiner.m_BoundingShape2D = null;
        
        // Find fresh bounds in current scene
        Collider2D newBounds = FindSceneCameraBounds();
        
        if (newBounds != null)
        {
            confiner.m_BoundingShape2D = newBounds;
            confiner.InvalidatePathCache();
            Debug.Log($"Camera bounds updated to: {newBounds.name}");
        }
        else
        {
            Debug.LogError($"No CameraBounds found in {SceneManager.GetActiveScene().name}!");
        }
    }

    Collider2D FindSceneCameraBounds()
    {
        // Priority 1: Exact name match
        GameObject boundsObject = GameObject.Find("CameraBounds");
        if (boundsObject != null) return boundsObject.GetComponent<Collider2D>();

        // Priority 2: Tag search
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        foreach (Collider2D col in allColliders)
        {
            if (col.CompareTag("CameraBounds")) return col;
        }

        return null;
    }

    void StorePlayerPosition()
    {
        if (playerTransform != null)
        {
            lastPlayerPosition = playerTransform.position;
            hasStoredPosition = true;
            Debug.Log($"Stored player position: {lastPlayerPosition}");
        }
    }

    void RestorePlayerPosition()
    {
        if (hasStoredPosition && IsReturningFromMenu() && playerTransform != null)
        {
            playerTransform.position = lastPlayerPosition;
            Debug.Log($"Restored player position: {lastPlayerPosition}");
            
            // Force camera to update immediately
            if (virtualCamera != null)
            {
                virtualCamera.ForceCameraPosition(
                    playerTransform.position, 
                    virtualCamera.transform.rotation
                );
            }
        }
    }

    void FindPlayer()
    {
        PlayerController player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            playerTransform = player.transform;
            if (virtualCamera != null) virtualCamera.Follow = playerTransform;
            Debug.Log($"Player reference updated: {playerTransform.name}");
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }
    }

    bool IsReturningFromMenu() => 
        previousScene == "LevelPicker" || 
        previousScene == "Meniu" || 
        previousScene == "CollectablesMenu";

    // Public methods for external access
    public void ForceRefreshCameraBounds() => UpdateCameraBounds();
    public void UpdatePlayerReference() => FindPlayer();
}