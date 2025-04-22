using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GoToSettings : MonoBehaviour
{
    public void OpenLevel()
    {
        SceneManager.LoadScene("Settings");
    }
}
