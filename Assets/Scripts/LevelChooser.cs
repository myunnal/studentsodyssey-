using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class LevelChooser : MonoBehaviour
{
    public Button[] button;
    public GameObject levelButtons;

    private void Awake()
    {
        ButtonToArray();
        int unlockedLevel = PlayerPrefs.GetInt("UnlockedLevel", 1);

        for (int i = 0; i < button.Length; i++)
        {
            button[i].interactable = false;
        }

        for (int i = 0; i < unlockedLevel; i++)
        {
            if (i < button.Length)
            {
                button[i].interactable = true;
            }
        }
    }

    public void OpenLevel(int levelID)
    {
        int buildIndex = levelID + 1;
        SceneManager.LoadScene(buildIndex);

    }

    void ButtonToArray()
    {
        int childCount = levelButtons.transform.childCount;
        button = new Button[childCount];
        for(int i  = 0; i < childCount; i++)
        {
            button[i] = levelButtons.transform.GetChild(i).gameObject.GetComponent<Button>();
        }
    }
}
