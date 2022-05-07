using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour {
    public AudioMixer audioMixer;
    Resolution[] resolutions;
    Display[] displays;
    [SerializeField] TMP_Dropdown ResolutionDropdown;
    Dictionary<int, int> resolutionDict = new Dictionary<int, int>();
    private void OnEnable() {
        LoadScreenResolutions();
    }
    private void LoadScreenResolutions() {
        resolutions = Screen.resolutions;
        resolutionDict.Clear();
        ResolutionDropdown.ClearOptions();
        List<string> resolutionList = new List<string>();
        Vector2Int lastRes = Vector2Int.zero;
        for (int i = 0; i < resolutions.Length; i++) {
            if ((lastRes.x != resolutions[i].width || lastRes.y != resolutions[i].height) && (resolutions[i].width / (float)resolutions[i].height > 1.7f && resolutions[i].width / (float)resolutions[i].height < 1.8f)) {
                string resolution = resolutions[i].width + " x " + resolutions[i].height;
                resolutionList.Add(resolution);
                resolutionDict.Add(resolutionList.Count - 1, i);
                lastRes = new Vector2Int(resolutions[i].width, resolutions[i].height);
            }
        }

        ResolutionDropdown.AddOptions(resolutionList);
    }
    public void ToggleFullscreen(bool boolean) {
        Screen.fullScreen = boolean;
        if (boolean) {
            LoadScreenResolutions();
            Screen.SetResolution(resolutions[resolutions.Length - 1].width, resolutions[resolutions.Length - 1].height, true);
        }        
    }
    public void ToggleVsync(bool boolean) => QualitySettings.vSyncCount = boolean ? 1 : 0;
    public void ChangeResolution(TMP_Dropdown target) {
        Screen.SetResolution(resolutions[resolutionDict[target.value]].width, resolutions[resolutionDict[target.value]].height, Screen.fullScreen);            
    }
}
