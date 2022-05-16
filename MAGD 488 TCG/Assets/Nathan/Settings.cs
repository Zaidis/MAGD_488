using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Settings : MonoBehaviour {
    public AudioMixer audioMixer;
    UnityEngine.UI.Toggle fsToggle;
    UnityEngine.UI.Toggle vsToggle;
    UnityEngine.UI.Slider audioSlider;
    List<Resolution> resolutions = new List<Resolution>();
    Display[] displays;
    
    [SerializeField] TMP_Dropdown ResolutionDropdown;
    Dictionary<int, int> resolutionDict = new Dictionary<int, int>();
    private void OnEnable() {        
        audioMixer = FindObjectOfType<AudioSource>().outputAudioMixerGroup.audioMixer;
        foreach(Selectable s in Selectable.allSelectablesArray){
            if(s.name == "VSync")
                vsToggle = s as UnityEngine.UI.Toggle;
            if(s.name == "ToggleFullscreen")
                fsToggle = s as UnityEngine.UI.Toggle;
            if(s.name == "VolumeSlider")
                audioSlider = s as UnityEngine.UI.Slider;
        }
        fsToggle.isOn = Screen.fullScreen;
        vsToggle.isOn = QualitySettings.vSyncCount > 0 ? true : false;
        audioSlider.value = PlayerPrefs.GetFloat("volume");
        audioMixer.SetFloat("MasterVolume", PlayerPrefs.GetFloat("volume"));
        LoadScreenResolutions();
    }
    private void LoadScreenResolutions() {
        resolutions = new List<Resolution>(Screen.resolutions);
        resolutionDict.Clear();
        ResolutionDropdown.ClearOptions();
        List<string> resolutionList = new List<string>();
        Vector2Int lastRes = Vector2Int.zero;
        for (int i = 0; i < resolutions.Count; i++) {
            if ((lastRes.x != resolutions[i].width || lastRes.y != resolutions[i].height) && (resolutions[i].width / (float)resolutions[i].height > 1.7f && resolutions[i].width / (float)resolutions[i].height < 1.8f)) {
                string resolution = resolutions[i].width + " x " + resolutions[i].height;
                resolutionList.Add(resolution);
                resolutionDict.Add(resolutionList.Count - 1, i);
                lastRes = new Vector2Int(resolutions[i].width, resolutions[i].height);
            }
        }
        ResolutionDropdown.AddOptions(resolutionList);
        int key = resolutionDict.First(x => x.Value == resolutions.FindIndex(r => r.width == Display.main.renderingWidth && r.height == Display.main.renderingHeight)).Key;
        ResolutionDropdown.value = key;
    }
    public void SetAudioLevel(float value) {
        if (value == -20)
            value = -80;
        audioMixer.SetFloat("MasterVolume", value);
        PlayerPrefs.SetFloat("volume", value);
        PlayerPrefs.Save();
    }
    public void ToggleFullscreen(bool boolean) {
        Screen.fullScreen = boolean;
        if (boolean) {
            LoadScreenResolutions();
            Screen.SetResolution(resolutions[resolutions.Count - 1].width, resolutions[resolutions.Count - 1].height, true);
        }        
    }
    public void ToggleVsync(bool boolean) => QualitySettings.vSyncCount = boolean ? 1 : 0;
    public void ChangeResolution(TMP_Dropdown target) {
        Screen.SetResolution(resolutions[resolutionDict[target.value]].width, resolutions[resolutionDict[target.value]].height, Screen.fullScreen);            
    }
}
