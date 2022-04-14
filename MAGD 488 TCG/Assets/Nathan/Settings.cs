using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Settings : MonoBehaviour
{
    public AudioMixer audioMixer;
    Resolution[] resolutions;

    public void ToggleFullscreen(bool boolean) => Screen.fullScreen = boolean;
}
