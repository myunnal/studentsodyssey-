using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockedNewLevel();
            SceneManager.LoadScene("LevelPicker");
        }
    }

    void UnlockedNewLevel()
    {
        int currentLevelBuildIndex = SceneManager.GetActiveScene().buildIndex;
        int currentLevelNumber = currentLevelBuildIndex - 1; 

        int reachedIndex = PlayerPrefs.GetInt("ReachedIndex", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevelNumber >= reachedIndex)
        {
            PlayerPrefs.SetInt("ReachedIndex", currentLevelNumber + 1);
        }

        if (currentLevelNumber >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel + 1);
        }

        PlayerPrefs.Save();
    }
}
