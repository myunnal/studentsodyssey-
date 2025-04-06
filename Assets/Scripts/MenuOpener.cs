using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuChooser : MonoBehaviour
{
    // Store the previous scene's name
    public static string previousScene;

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

    public void OpenMenu(int levelID)
    {
        string levelName = "Level" + levelID;
        SceneManager.LoadScene(levelName);
    }
    
    public void OpenCollectablesMenu()
    {
        // Save the current scene before switching to the menu
        previousScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene("CollectablesMenu"); 
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            // Check if we're currently in the CollectablesMenu scene
            if (SceneManager.GetActiveScene().name == "CollectablesMenu")
            {
                // Close the menu by loading the previous scene
                SceneManager.LoadScene(previousScene);
            }
            else
            {
                // Open the menu
                OpenCollectablesMenu();
            }
        }
    }
}
