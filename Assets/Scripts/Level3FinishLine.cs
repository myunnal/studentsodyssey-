using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; 
using System.Collections;

public class Level3FinishLine : MonoBehaviour
{
    public TextMeshProUGUI messageText;
    public float messageDuration = 2.5f;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            int collectedCount = InventoryManager.Instance.GetCollectedItems().Count;

            if (collectedCount >= 9)
            {
                SceneManager.LoadScene("Ending");
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
            messageText.text = "-Blemba að dar ne viskà surinkau";
            messageText.enabled = true;
        }

        yield return new WaitForSeconds(messageDuration);

        SceneManager.LoadScene("meniu");
    }

}
