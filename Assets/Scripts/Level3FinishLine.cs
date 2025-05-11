using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class Level3FinishLine : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float messageDuration = 2.5f;
    public AudioClip finishSound; // 👈 Add this
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int collectedCount = InventoryManager.Instance.GetCollectedItems().Count;

            if (finishSound != null)
            {
                audioSource.PlayOneShot(finishSound);
            }

            if (collectedCount >= 9)
            {
                StartCoroutine(LoadSceneAfterDelay("Ending", finishSound.length));
            }
            else
            {
                StartCoroutine(ShowMessageAndReturnToMenu());
            }
        }
    }

    private IEnumerator ShowMessageAndReturnToMenu()
    {
        if (messageText != null)
        {
            messageText.text = "-Blemba aš dar ne viską surinkau";
            messageText.enabled = true;
        }

        yield return new WaitForSeconds(messageDuration);

        SceneManager.LoadScene("meniu");
    }

    private IEnumerator LoadSceneAfterDelay(string sceneName, float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene(sceneName);
    }
}
