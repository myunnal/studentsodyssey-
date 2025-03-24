using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : MonoBehaviour
{
    public static BackgroundMusicManager instance;

    private AudioSource audioSource;

    public AudioClip defaultMusic;
    public AudioClip level1Music;

    private void Awake()
    {
        // Ensure only one instance of BackgroundMusicManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // Keep the music playing across scenes
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        audioSource = GetComponent<AudioSource>();

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // This method is called whenever a new scene is loaded
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Scene loaded: " + scene.name);

        // Stop the music if Level1 is loaded
        if (scene.name == "Level1")
        {
            Debug.Log("Switching to Level1 music...");
            PlayMusic(level1Music);
        }
        else
        {
            Debug.Log("Switching to default music...");
            PlayMusic(defaultMusic);
        }
    }

    public void PlayMusic(AudioClip musicClip)
    {
        if (audioSource.clip != musicClip || !audioSource.isPlaying)
        {
            Debug.Log("Playing music: " + musicClip.name);
            audioSource.clip = musicClip;
            audioSource.Play();
        }
    }

   

    public void StopMusic()
    {
        if (audioSource.isPlaying)
        {
            Debug.Log("Stopping background music");
            audioSource.Stop();
        }
    }

    private void OnDestroy()
    {
        // Unsubscribe from the sceneLoaded event to avoid memory leaks
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

