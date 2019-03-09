using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Rendering.PostProcessing;
using TMPro;

public class Setting : MonoBehaviour
{
    /// <summary>
    /// 分辨率
    /// </summary>
    public TMP_Dropdown ResolutionDropdown = null;
    public Toggle FullScreen = null;
    /// <summary>
    /// 垂直同步30
    /// </summary>
    public Toggle VSync30 = null;
    /// <summary>
    /// 垂直同步60
    /// </summary>
    public Toggle VSync60 = null;
    /// <summary>
    /// 画质
    /// </summary>
    public TMP_Dropdown QualityDropdown = null;
    /// <summary>
    /// 亮度
    /// </summary>
    public Slider BrightnessSlider = null;
    /// <summary>
    /// BGM
    /// </summary>
    public Slider BGMSlider = null;
    /// <summary>
    /// 效果音
    /// </summary>
    public Slider SESlider = null;
    /// <summary>
    /// 音量上下限
    /// </summary>
    public Vector2 VolumeLimit = new Vector2(-40f, 0f);

    public PostProcessProfile PostProcess = null;
    public AudioMixer Mixer = null;

    private float tempBrightness = 0.5f;
    private float tempBGM = 1f;
    private float tempSE = 1f;

    public event Action OnClose = null;
    public List<Resolution> Resolutions { get; set; } = null;
    public AutoExposure Exposure { get; set; } = null;


    public void Accept()
    {
        Screen.SetResolution(this.Resolutions[this.ResolutionDropdown.value].width, this.Resolutions[this.ResolutionDropdown.value].height, this.FullScreen.isOn);
        if (this.VSync30.isOn)
            QualitySettings.vSyncCount = 2;
        else if (this.VSync60.isOn)
            QualitySettings.vSyncCount = 1;
        else QualitySettings.vSyncCount = 0;
        // 画质
        QualitySettings.SetQualityLevel(this.QualityDropdown.value);
        Close();
    }
    public void Cancle()
    {
        SetBrightness(this.tempBrightness);
        SetBGM(this.tempBGM);
        SetSE(this.tempSE);
        Close();
    }
    public void SetBrightness(float value)
    {
        this.Exposure.keyValue.value = FunctionExtension.Remap(value, 0f, 1f, 0f, 2f);
    }
    public void SetBGM(float value)
    {
        this.Mixer.SetFloat("BGMvolume", FunctionExtension.Remap(value, 0f, 1f, this.VolumeLimit.x, this.VolumeLimit.y));
    }
    public void SetSE(float value)
    {
        this.Mixer.SetFloat("SEvolume", FunctionExtension.Remap(value, 0f, 1f, this.VolumeLimit.x, this.VolumeLimit.y));
    }

    public void Open()
    {
        this.gameObject.SetActive(true);

        // 解像度
        {
            var rs = new List<string>();
            var index = 0;
            for (var i = 0; i < this.Resolutions.Count; i++)
            {
                rs.Add($"{this.Resolutions[i].width}x{this.Resolutions[i].height}");
                if (this.Resolutions[i].Equal(Screen.currentResolution))
                    index = i;
            }
            this.ResolutionDropdown.ClearOptions();
            this.ResolutionDropdown.AddOptions(rs);
            this.ResolutionDropdown.value = index;
        }

        this.FullScreen.isOn = Screen.fullScreen;

        // 垂直同步
        switch (QualitySettings.vSyncCount)
        {
            case 0:
                this.VSync30.isOn = false;
                this.VSync60.isOn = false;
                break;
            case 1:
                this.VSync30.isOn = false;
                this.VSync60.isOn = true;
                break;
            case 2:
                this.VSync30.isOn = true;
                this.VSync60.isOn = false;
                break;
            default: break;
        }

        // 画质
        {
            var qs = new List<string>(QualitySettings.names);
            this.QualityDropdown.ClearOptions();
            this.QualityDropdown.AddOptions(qs);
            this.QualityDropdown.value = QualitySettings.GetQualityLevel();
        }

        this.BrightnessSlider.value = FunctionExtension.Remap(this.Exposure.keyValue.value, 0f, 2f, 0f, 1f);
        this.tempBrightness = this.BrightnessSlider.value;

        var volume = 0f;
        this.Mixer.GetFloat("BGMvolume", out volume);
        this.BGMSlider.value = FunctionExtension.Remap(volume, this.VolumeLimit.x, this.VolumeLimit.y, 0f, 1f);
        this.tempBGM = this.BGMSlider.value;

        volume = 0f;
        this.Mixer.GetFloat("SEvolume", out volume);
        this.SESlider.value = FunctionExtension.Remap(volume, this.VolumeLimit.x, this.VolumeLimit.y, 0f, 1f);
        this.tempSE = this.SESlider.value;
    }
    public void Close()
    {
        AudioManager.Instance.Play("Click2");
        OnClose?.Invoke();
        this.gameObject.SetActive(false);
    }

    private void Awake()
    {
        this.Resolutions = new List<Resolution>();
        foreach (var resolution in Screen.resolutions)
        {
            if (Mathf.Approximately((float)resolution.width / resolution.height, 16f / 9f))
            {
                if (this.Resolutions.FindIndex(r => r.Equal(resolution)) < 0)
                    this.Resolutions.Add(resolution);
            }
        }

        if (this.Exposure == null)
            this.Exposure = this.PostProcess.GetSetting<AutoExposure>();
    }
}
