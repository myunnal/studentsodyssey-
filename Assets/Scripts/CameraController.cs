using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraController : MonoBehaviour
{
    [Header("Target Settings")]
    public Transform target;
    public float smoothSpeed = 0.125f;
    public Vector3 offset = new Vector3(0, 2, -10);

    [Header("Boundaries")]
    public bool useBoundaries = false;
    public float minX = -10f;
    public float maxX = 10f;
    public float minY = -5f;
    public float maxY = 5f;

    private static CameraController instance;
    private Vector3 lastKnownPlayerPosition;

    void Awake()
    {
        // Singleton pattern to ensure only one camera controller exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // If we're in menu scenes, hide the camera but don't destroy it
        if (scene.name == "LevelPicker" || scene.name == "Meniu" || scene.name == "CollectablesMenu")
        {
            // Make the camera inactive but don't destroy it
            gameObject.SetActive(false);
            return;
        }
        else
        {
            // Reactivate the camera when returning to gameplay scenes
            gameObject.SetActive(true);
            
            // Find the player in the new scene
            FindPlayer();
        }
    }

    void FindPlayer()
    {
        // Look for the player in the scene
        PlayerController playerController = FindObjectOfType<PlayerController>();
        if (playerController != null)
        {
            target = playerController.transform;
            
            // If we have a last known position and it's not the default Vector3.zero
            if (lastKnownPlayerPosition != Vector3.zero)
            {
                // Immediately update camera position to avoid jarring movement
                transform.position = lastKnownPlayerPosition + offset;
            }
        }
        else
        {
            Debug.LogWarning("No player found in the scene!");
        }
    }

    void LateUpdate()
    {
        if (target == null)
        {
            FindPlayer();
            return;
        }

        // Calculate the desired position
        Vector3 desiredPosition = target.position + offset;
        
        // Apply boundaries if enabled
        if (useBoundaries)
        {
            desiredPosition.x = Mathf.Clamp(desiredPosition.x, minX, maxX);
            desiredPosition.y = Mathf.Clamp(desiredPosition.y, minY, maxY);
        }
        
        // Smoothly move towards the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        transform.position = smoothedPosition;
        
        // Store the last known position for scene transitions
        lastKnownPlayerPosition = transform.position - offset;
    }
}