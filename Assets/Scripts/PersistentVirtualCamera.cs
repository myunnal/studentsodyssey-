using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class PersistentVirtualCamera : MonoBehaviour
{
    // Static reference to maintain singleton
    private static PersistentVirtualCamera instance;
    
    // Reference to the virtual camera component
    private CinemachineVirtualCamera virtualCamera;
    
    // Reference to the confiner component
    private CinemachineConfiner confiner;
    
    // Player tracking
    private Transform playerTransform;
    
    // Scene transition tracking
    private static Vector3 lastPlayerPosition;
    private static bool hasStoredPosition = false;
    private static string previousScene = "";
    
    // State management
    private bool isInMenuScene = false;
    
    // Store confiner settings by scene name
    private class ConfinerData
    {
        public string sceneName;
        public string boundingObjectName;
    }
    
    // Cache of confiner settings for each scene
    private static System.Collections.Generic.Dictionary<string, ConfinerData> confinerDataByScene = 
        new System.Collections.Generic.Dictionary<string, ConfinerData>();

    void Awake()
    {
        // Implement singleton pattern
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            
            // Get components
            virtualCamera = GetComponent<CinemachineVirtualCamera>();
            confiner = GetComponent<CinemachineConfiner>();
            
            if (virtualCamera == null)
            {
                Debug.LogError("No CinemachineVirtualCamera found!");
            }
            
            if (confiner == null)
            {
                Debug.LogWarning("No CinemachineConfiner found. Camera bounds won't be applied.");
            }
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    
    void Start()
    {
        // Store initial player reference
        if (virtualCamera != null && virtualCamera.Follow != null)
        {
            playerTransform = virtualCamera.Follow;
            Debug.Log("Initial player reference set: " + playerTransform.name);
        }
        else
        {
            FindPlayer();
        }
        
        // Store initial confiner data
        StoreCurrentConfinerData();
    }
    
    void OnDestroy()
    {
        if (instance == this)
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
    }
    
    void StoreCurrentConfinerData()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Skip menu scenes
        if (currentScene == "LevelPicker" || currentScene == "Meniu" || currentScene == "CollectablesMenu")
        {
            return;
        }
        
        // Store confiner bounding shape info for this scene
        if (confiner != null && confiner.m_BoundingShape2D != null)
        {
            ConfinerData data = new ConfinerData
            {
                sceneName = currentScene,
                boundingObjectName = confiner.m_BoundingShape2D.name
            };
            
            // Save in dictionary
            confinerDataByScene[currentScene] = data;
            Debug.Log("Stored confiner data for scene " + currentScene + " with bounds object: " + data.boundingObjectName);
        }
    }
    
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string currentSceneName = scene.name;
        
        // Check if we're in a menu scene
        isInMenuScene = (currentSceneName == "LevelPicker" || 
                         currentSceneName == "Meniu" || 
                         currentSceneName == "CollectablesMenu");
        
        if (isInMenuScene)
        {
            // We're entering a menu scene
            if (playerTransform != null)
            {
                // Store player position before entering menu
                lastPlayerPosition = playerTransform.position;
                hasStoredPosition = true;
                Debug.Log("Stored player position before menu: " + lastPlayerPosition);
            }
            
            // Store confiner data before entering menu
            StoreCurrentConfinerData();
            
            // Disable the virtual camera in menu scenes
            if (virtualCamera != null)
            {
                gameObject.SetActive(false);
            }
        }
        else
        {
            // We're in a gameplay scene
            gameObject.SetActive(true);
            
            // Small delay to ensure all objects are loaded
            Invoke("HandleSceneTransition", 0.2f);
        }
        
        // Update previous scene
        previousScene = currentSceneName;
    }
    
    void HandleSceneTransition()
    {
        // Find player in the new scene
        FindPlayer();
        
        // Restore player position if coming from a menu
        if (playerTransform != null && hasStoredPosition)
        {
            if (previousScene == "LevelPicker" || 
                previousScene == "Meniu" || 
                previousScene == "CollectablesMenu")
            {
                // Only restore position when coming back from a menu
                playerTransform.position = lastPlayerPosition;
                Debug.Log("Restored player to position: " + lastPlayerPosition);
            }
        }
        
        // Find and set up the confiner for this scene
        UpdateConfiner();
    }
    
    void UpdateConfiner()
    {
        if (confiner == null) return;
        
        string currentScene = SceneManager.GetActiveScene().name;
        
        // Skip menu scenes
        if (currentScene == "LevelPicker" || currentScene == "Meniu" || currentScene == "CollectablesMenu")
        {
            return;
        }
        
        // Find the bounding shape in this scene
        Collider2D[] allColliders = FindObjectsOfType<Collider2D>();
        
        // First try to find the bounds object we used previously in this scene
        if (confinerDataByScene.ContainsKey(currentScene))
        {
            string targetName = confinerDataByScene[currentScene].boundingObjectName;
            
            foreach (Collider2D col in allColliders)
            {
                if (col.name == targetName)
                {
                    // Found the exact same bounds object
                    confiner.m_BoundingShape2D = col;
                    confiner.InvalidatePathCache();
                    Debug.Log("Restored previous confiner bounds: " + targetName);
                    return;
                }
            }
        }
        
        // If we can't find a previous match, look for any object named "CameraBounds"
        foreach (Collider2D col in allColliders)
        {
            if (col.name.Contains("CameraBounds") || col.name.Contains("Bounds") || col.gameObject.CompareTag("CameraBounds"))
            {
                confiner.m_BoundingShape2D = col;
                confiner.InvalidatePathCache();
                Debug.Log("Found new camera bounds: " + col.name);
                
                // Store for future reference
                ConfinerData data = new ConfinerData
                {
                    sceneName = currentScene,
                    boundingObjectName = col.name
                };
                confinerDataByScene[currentScene] = data;
                return;
            }
        }
        
        // If we can't find any suitable bounds, log a warning
        Debug.LogWarning("No camera bounds found in scene: " + currentScene);
    }
    
    void FindPlayer()
    {
        // Find the player in the scene
        PlayerController player = FindObjectOfType<PlayerController>();
        
        if (player != null)
        {
            playerTransform = player.transform;
            
            // Update the virtual camera's target
            if (virtualCamera != null)
            {
                virtualCamera.Follow = playerTransform;
                Debug.Log("Updated camera target to: " + playerTransform.name);
            }
        }
        else
        {
            Debug.LogWarning("Could not find player in scene!");
        }
    }
    
    // Can be called from outside to force update the player reference
    public void UpdatePlayerReference()
    {
        FindPlayer();
    }
    
    // Can be called to force update the confiner
    public void ForceUpdateConfiner()
    {
        UpdateConfiner();
    }
}