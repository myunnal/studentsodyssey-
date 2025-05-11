using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndLevel : MonoBehaviour
{
    public AudioClip finishSound; 
    private AudioSource audioSource; 

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>(); // Optional: reuse one already attached
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            UnlockedNewLevel();
            PlayFinishSoundAndLoadScene();
        }
    }

    void PlayFinishSoundAndLoadScene()
    {
        if (finishSound != null)
        {
            audioSource.PlayOneShot(finishSound);
        }
        // Optional delay if you want the sound to play before switching
        StartCoroutine(LoadSceneAfterDelay("LevelPicker", finishSound.length));
    }

    IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
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