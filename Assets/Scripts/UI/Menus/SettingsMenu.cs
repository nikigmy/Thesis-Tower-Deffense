using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    [Header("Scene Setup")]
    [Header("Graphics")]
    [SerializeField]
    GameObject fullScreenOn;
    [SerializeField]
    GameObject fullScreenOff;
    [SerializeField]
    Text resolution;
    [SerializeField]
    Text quality;

    [Header("Sound")]
    [SerializeField]
    Slider music;
    [SerializeField]
    Slider sfx;

    [Header("Controls")]
    [SerializeField]
    Slider cameraMoveSpeed;
    [SerializeField]
    Slider cameraZoomSpeed;
    [SerializeField]
    GameObject fastBuildingOn;
    [SerializeField]
    GameObject fastBuildingOff;

    private List<Vector2> Resolutions = new List<Vector2>()
    {
        new Vector2(640,400),
        new Vector2(640,480),
        new Vector2(800,600),
        new Vector2(1024,768),
        new Vector2(1152,864),
        new Vector2(1280,600),
        new Vector2(1280,720),
        new Vector2(1280,768),
        new Vector2(1280,800),
        new Vector2(1280,960),
        new Vector2(1280,1024),
        new Vector2(1360,768),
        new Vector2(1366,768),
        new Vector2(1440,1050),
        new Vector2(1440,900),
        new Vector2(1600,900),
        new Vector2(1680,1050),
        new Vector2(1920,1080),
    };

    private List<string> qualities = new List<string>() { "Very Low", "Low", "Medium", "High", "Very High", "Ultra" };

    int currentQualityIndex;
    int currentResolutionIndex;

    public void LoadSettings()
    {
        var settings = Def.Instance.Settings;
        //graphics
        if (settings.Fullscreen)
        {
            fullScreenOn.SetActive(true);
            fullScreenOff.SetActive(false);
        }
        else
        {
            fullScreenOn.SetActive(false);
            fullScreenOff.SetActive(true);
        }

        var currentRes = Resolutions.Find(x => x.x == settings.Resolution.x && x.y == settings.Resolution.y);
        if (currentRes != null)
        {
            currentResolutionIndex = Resolutions.IndexOf(currentRes);
        }
        else
        {
            Resolutions.Insert(0, settings.Resolution);
            currentResolutionIndex = 0;
        }
        resolution.text = Resolutions[currentResolutionIndex].x + "x" + Resolutions[currentResolutionIndex].y;

        if (settings.QualityLevel >= 0 && settings.QualityLevel <= 5)
        {
            currentQualityIndex = settings.QualityLevel;
        }
        else
        {
            currentQualityIndex = 5;
        }
        quality.text = qualities[currentQualityIndex];
        //sound
        music.value = settings.MusicLevel;
        sfx.value = settings.SFXLevel;

        //controls
        cameraMoveSpeed.value = settings.CameraMoveSpeed;
        cameraZoomSpeed.value = settings.CameraZoomSpeed;
        if (settings.FastBuilding)
        {
            fastBuildingOn.SetActive(true);
            fastBuildingOff.SetActive(false);
        }
        else
        {
            fastBuildingOn.SetActive(false);
            fastBuildingOff.SetActive(true);
        }
        gameObject.SetActive(true);
    }

    public void ChangeResolutionSetting(bool right)
    {
        if (right && currentResolutionIndex < Resolutions.Count - 1)
        {
            currentResolutionIndex++;
            resolution.text = Resolutions[currentResolutionIndex].x + "x" + Resolutions[currentResolutionIndex].y;
        }
        else if (!right && currentResolutionIndex > 0)
        {
            currentResolutionIndex--;
            resolution.text = Resolutions[currentResolutionIndex].x + "x" + Resolutions[currentResolutionIndex].y;
        }
    }

    public void ChangeQualitySetting(bool right)
    {
        if (right && currentQualityIndex < 5)
        {
            currentQualityIndex++;
            quality.text = qualities[currentQualityIndex];
        }
        else if (!right && currentQualityIndex > 0)
        {
            currentQualityIndex--;
            quality.text = qualities[currentQualityIndex];
        }
    }

    public void Close()
    {
        var settings = Def.Instance.Settings;
        bool changed = false;
        //graphics
        if(fullScreenOn.activeSelf != settings.Fullscreen)
        {
            settings.Fullscreen = fullScreenOn.activeSelf;
            changed = true;
        }
        if (resolution.text != settings.Resolution.x + "x" + settings.Resolution.y)
        {
            settings.Resolution = Resolutions[currentResolutionIndex];
            changed = true;
        }
        if (currentQualityIndex != settings.QualityLevel)
        {
            settings.QualityLevel = currentQualityIndex;
            changed = true;
        }

        //sound
        if (music.value != settings.MusicLevel)
        {
            settings.MusicLevel = (int)music.value;
            changed = true;
        }
        if (sfx.value != settings.SFXLevel)
        {
            settings.SFXLevel = (int)sfx.value;
            changed = true;
        }

        //controls
        if (cameraMoveSpeed.value != settings.CameraMoveSpeed)
        {
            settings.CameraMoveSpeed = (int)cameraMoveSpeed.value;
            changed = true;
        }
        if (cameraZoomSpeed.value != settings.CameraZoomSpeed)
        {
            settings.CameraZoomSpeed = (int)cameraZoomSpeed.value;
            changed = true;
        }
        if (fastBuildingOn.activeSelf != settings.FastBuilding)
        {
            settings.FastBuilding = fastBuildingOn.activeSelf;
            changed = true;
        }

        if (changed)
        {
            Helpers.SaveAndSetSettings();
        }
        gameObject.SetActive(false);
    }
}
