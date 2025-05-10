using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuChooser : MonoBehaviour
{
    // Store the previous scene's name
    public static string previousScene;
    
    // Store player position before menu
    public static Vector3 playerPositionBeforeMenu;
    
    // Reference to the main camera to track its confines
    private static CameraConfinesTracker cameraTracker;

    private static MenuChooser instance;

    private void Awake()
    {
        // Implementing a singleton pattern for persistence
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Find and store the camera confines tracker if it exists
        if (cameraTracker == null)
        {
            cameraTracker = FindObjectOfType<CameraConfinesTracker>();
        }
    }

    public void OpenMenu(int levelID)
    {
        string levelName = "Level" + levelID;
        SceneManager.LoadScene(levelName);
    }
    
    public void OpenCollectablesMenu()
    {
        // Save the current scene before switching to the menu
        previousScene = SceneManager.GetActiveScene().name;
        
        // Find and store the player position
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            playerPositionBeforeMenu = player.transform.position;
            Debug.Log("Stored player position: " + playerPositionBeforeMenu);
        }
        
        // Store camera confines if tracker exists
        if (cameraTracker != null)
        {
            cameraTracker.StoreConfines();
        }
        
        SceneManager.LoadScene("CollectablesMenu"); 
    }
    
    public void ReturnFromMenu()
    {
        SceneManager.LoadScene(previousScene);
        
        // Add a post-load check for camera boundaries
        StartCoroutine(CheckCameraAfterLoad());
    }
    
    private System.Collections.IEnumerator CheckCameraAfterLoad()
    {
        // Wait for scene to load fully
        yield return new WaitForSeconds(1f);
        
        // Try to find the camera tracker and force restore
        CameraConfinesTracker tracker = FindObjectOfType<CameraConfinesTracker>();
        if (tracker != null)
        {
            tracker.ForceRestoreConfines();
            Debug.Log("Forced camera confines restoration after menu return");
        }
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Check if we're currently in the CollectablesMenu scene
            if (SceneManager.GetActiveScene().name == "CollectablesMenu")
            {
                // Close the menu by loading the previous scene
                ReturnFromMenu();
            }
            else
            {
                // Open the menu
                OpenCollectablesMenu();
            }
        }
    }
}