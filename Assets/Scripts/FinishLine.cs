using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            UnlockedNewLevel();
            SceneManager.LoadScene("LevelPicker");
        }
    }

    void UnlockedNewLevel()
    {
        int currentLevel = SceneManager.GetActiveScene().buildIndex;
        int reachedLevel = PlayerPrefs.GetInt("ReachedIndex", 1);
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        if (currentLevel >= reachedLevel)
        {
            PlayerPrefs.SetInt("ReachedIndex", currentLevel + 1);
        }

        if (currentLevel >= unlockedLevel)
        {
            PlayerPrefs.SetInt("UnlockedLevel", unlockedLevel + 1);
        }

        PlayerPrefs.Save();
    }
}
