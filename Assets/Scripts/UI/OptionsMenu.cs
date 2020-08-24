﻿using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;

namespace IntelligentCake.UI
{
    public class OptionsMenu : MonoBehaviour
    {
        public AudioMixer audioMixer;

        public TMP_Dropdown resolutionDropdown;

        private Resolution[] _resolutions;
        private void Start()
        {
            _resolutions = Screen.resolutions.Select(resolution => new Resolution { width = resolution.width, height = resolution.height }).Distinct().ToArray();
            
            resolutionDropdown.ClearOptions();
            
            List<string> options = new List<string>();

            int currentResolutionIndex = 0;
            for (int i = 0; i < _resolutions.Length; i++)
            {
                string option = _resolutions[i].width + " x " + _resolutions[i].height;
                options.Add(option);

                if (_resolutions[i].width == Screen.width && _resolutions[i].height == Screen.height)
                {
                    currentResolutionIndex = i;
                }
            }
            
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = currentResolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }

        public void SetResolution(int resolutionIndex)
        {
            Resolution resolution = _resolutions[resolutionIndex];
            Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetVolume(float volume)
        {
            audioMixer.SetFloat("volume", Mathf.Log10(volume) * 20); 
        }

        public void SetQuality(int qualityIndex)
        {
            QualitySettings.SetQualityLevel(qualityIndex);
        }

        public void SetFullscreen(bool isFullscreen)
        {
            Screen.fullScreen = isFullscreen;
        }
    }
}
