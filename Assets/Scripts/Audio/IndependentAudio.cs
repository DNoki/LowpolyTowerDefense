using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

/// <summary>
/// 独立音源
/// 独立オーディオ
/// </summary>
[System.Serializable]
public class IndependentAudio
{
    public string Name = string.Empty;
    public AudioClip Clip = null;
    public AudioMixerGroup AMG = null;

    [Range(.1f, 3f)] public float Pitch = 1f;
    [Range(0f, 1f)] public float Volume = 1f;
    public bool IsLoop = false;

    public AudioSource Source { get; set; }
}