using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeSceneAndPlayAudio : MonoBehaviour
{
    public AudioSource audioSource;

    public void startGame()
    {
        if (audioSource != null)
        {
            StartCoroutine(PlayAudioThenLoadScene(audioSource));
        }
        else
        {
            SceneManager.LoadScene("LevelPicker");
        }
    }

    private IEnumerator PlayAudioThenLoadScene(AudioSource source)
    {
        source.Play();
        while (source.isPlaying)
        {
            yield return null;
        }
        SceneManager.LoadScene("LevelPicker");
    }


    public void leaveGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the Editor
#endif
        Application.Quit(); 
       
    }
}
