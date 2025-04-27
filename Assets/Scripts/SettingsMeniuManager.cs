using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;using UnityEngine.Audio;

public class SettingsMeniuManager : MonoBehaviour
{
    public AudioMixer MasterAudio;


    public TMP_Dropdown ResDropDown;
    public Toggle FullScreen;

    Resolution[] AllRez;
    bool isFullScreen;
    int SelectedResolution;

    List<Resolution> SelectedRezList = new List<Resolution>();

    private void Start()
    {

        isFullScreen = true;
        AllRez = Screen.resolutions;

        List<string> resolutionStringList = new List<string>();
        string newRez;

        foreach(Resolution res in AllRez)
        {
            newRez = res.width.ToString() + " x " + res.height.ToString();
            if(!resolutionStringList.Contains(newRez))
            {
                resolutionStringList.Add(newRez);
                SelectedRezList.Add(res);
            }
        }

        ResDropDown.AddOptions(resolutionStringList);
    }

    public void ChangeResolution()
    {
        SelectedResolution = ResDropDown.value;
        Screen.SetResolution(SelectedRezList[SelectedResolution].width, SelectedRezList[SelectedResolution].height, isFullScreen);
    }

    public void ChangeFullScreen()
    {
        isFullScreen = FullScreen.isOn;
        Screen.SetResolution(SelectedRezList[SelectedResolution].width, SelectedRezList[SelectedResolution].height, isFullScreen);
    }

    public void SetVolume(float volume)
    {
        MasterAudio.SetFloat("volume", volume);
    }

}
