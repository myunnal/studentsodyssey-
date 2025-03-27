using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuChooser : MonoBehaviour
{
    public void OpenMenu(int levelID)
    {
        string levelName = "Level" + levelID;
        SceneManager.LoadScene(levelName);
    }
    
    public void OpenCollectablesMenu()
    {
        SceneManager.LoadScene("CollectablesMenu"); // Ensure the scene name matches exactly
    }
    
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            OpenCollectablesMenu();
        }
    }
}
