using UnityEngine;
using Cinemachine;
using UnityEngine.SceneManagement;

public class CameraConfinesTracker : MonoBehaviour
{
    // Reference to the virtual camera
    private CinemachineVirtualCamera virtualCamera;
    
    // Reference to the confiner component
    private CinemachineConfiner confiner;
    
    // Store the confiner collider
    private Collider2D storedConfinerCollider;
    
    // Flag to check if we're returning from a menu
    private bool isReturningFromMenu = false;

    private void Awake()
    {
        // Keep this object across scene changes
        DontDestroyOnLoad(gameObject);
        
        // Subscribe to scene loaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // Clean up subscription
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Start()
    {
        // Find the virtual camera and confiner on start
        FindCameraComponents();
    }
    
    private void FindCameraComponents()
    {
        // Find the virtual camera - might be multiple cameras, find all of them
        CinemachineVirtualCamera[] allCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        
        if (allCameras.Length > 0)
        {
            // Use the first active camera
            foreach (var cam in allCameras)
            {
                if (cam.gameObject.activeInHierarchy)
                {
                    virtualCamera = cam;
                    break;
                }
            }
            
            // If no active camera found, just use the first one
            if (virtualCamera == null && allCameras.Length > 0)
            {
                virtualCamera = allCameras[0];
            }
            
            // Get the confiner component if the virtual camera exists
            if (virtualCamera != null)
            {
                // Check for confiner directly on the camera
                confiner = virtualCamera.GetComponent<CinemachineConfiner>();
                
                // If not found, it might be on a parent or child
                if (confiner == null)
                {
                    confiner = virtualCamera.GetComponentInParent<CinemachineConfiner>();
                }
                
                if (confiner == null)
                {
                    confiner = virtualCamera.GetComponentInChildren<CinemachineConfiner>();
                }
                
                // If still not found, search the entire scene (might be on a separate object)
                if (confiner == null)
                {
                    confiner = FindObjectOfType<CinemachineConfiner>();
                }
                
                if (confiner != null && confiner.m_BoundingShape2D != null)
                {
                    Debug.Log("Found camera confiner with bounds: " + confiner.m_BoundingShape2D.name);
                }
                else if (confiner != null)
                {
                    Debug.Log("Found confiner but bounds are missing");
                }
                else
                {
                    Debug.LogWarning("Camera confiner component not found!");
                }
            }
            else
            {
                Debug.LogWarning("No active virtual camera found in scene!");
            }
        }
        else
        {
            Debug.LogWarning("No virtual cameras found in scene!");
        }
    }
    
    // Method to store the current camera confines before switching scenes
    public void StoreConfines()
    {
        if (confiner != null && confiner.m_BoundingShape2D != null)
        {
            storedConfinerCollider = confiner.m_BoundingShape2D;
            Debug.Log("Stored camera confines: " + storedConfinerCollider.name);
        }
        
        isReturningFromMenu = true;
    }
    
    // Method to restore camera confines
    public void RestoreConfines()
    {
        // Wait a bit longer to ensure all objects are loaded properly
        Invoke("ActuallyRestoreConfines", 0.2f);
    }
    
    // Actual restoration logic separated to allow for delay
    private void ActuallyRestoreConfines()
    {
        // Re-find the camera components as they might have changed with scene loading
        FindCameraComponents();
        
        if (confiner != null)
        {
            if (storedConfinerCollider != null)
            {
                // Try to find a matching collider in the new scene by name
                Collider2D[] colliders = FindObjectsOfType<Collider2D>();
                foreach (Collider2D col in colliders)
                {
                    // Look for a collider with the same name
                    if (col != null && storedConfinerCollider != null && 
                        col.name == storedConfinerCollider.name)
                    {
                        confiner.m_BoundingShape2D = col;
                        Debug.Log("Restored camera confines to: " + col.name);
                        
                        // Inform Cinemachine that the confiner has changed
                        try {
                            confiner.InvalidatePathCache();
                        } catch (System.Exception e) {
                            Debug.LogWarning("Failed to invalidate path cache: " + e.Message);
                        }
                        
                        break;
                    }
                }
                
                // If we didn't find a matching name, just set what we have
                if (confiner.m_BoundingShape2D == null && storedConfinerCollider != null)
                {
                    confiner.m_BoundingShape2D = storedConfinerCollider;
                    try {
                        confiner.InvalidatePathCache();
                    } catch (System.Exception e) {
                        Debug.LogWarning("Failed to invalidate path cache: " + e.Message);
                    }
                    Debug.Log("Restored camera confines using stored reference");
                }
            }
            else
            {
                // Try to find any active boundary in the scene
                Collider2D[] boundaries = FindObjectsOfType<Collider2D>();
                foreach (Collider2D boundary in boundaries)
                {
                    // Look for likely boundary names
                    if (boundary.name.ToLower().Contains("bound") || 
                        boundary.name.ToLower().Contains("confine") ||
                        boundary.name.ToLower().Contains("limit") ||
                        boundary.gameObject.layer == LayerMask.NameToLayer("ConfineLayer"))
                    {
                        confiner.m_BoundingShape2D = boundary;
                        Debug.Log("Found and set likely camera boundary: " + boundary.name);
                        try {
                            confiner.InvalidatePathCache();
                        } catch (System.Exception e) {
                            Debug.LogWarning("Failed to invalidate path cache: " + e.Message);
                        }
                        break;
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Cannot restore confines because no confiner component was found!");
        }
        
        isReturningFromMenu = false;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Skip setup for menu scenes
        if (scene.name == "CollectablesMenu" || scene.name == "LevelPicker" || scene.name == "Meniu")
        {
            return;
        }
            
        // Check if we're returning from a menu
        if (isReturningFromMenu)
        {
            // Longer delay to ensure all objects are loaded
            Invoke("RestoreConfines", 0.5f);
        }
        else
        {
            // In case we're just loading a new scene, find components
            Invoke("FindCameraComponents", 0.5f); 
        }
    }
    
    // Call this method from other scripts if camera is still not working
    public void ForceRestoreConfines()
    {
        StopAllCoroutines();
        CancelInvoke();
        RestoreConfines();
    }
}