using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Audio : MonoBehaviour
{
    public List<AudioSource> Sources { get; set; } = null;

    public void PlayRandomAudio()
    {
        var audio = this.Sources[Random.Range(0, this.Sources.Count)];
        audio.Play();
    }
    public void StopAllAudio()
    {
        foreach (var audio in this.Sources)
        {
            audio.Stop();
        }
    }

    private void Awake()
    {
        this.Sources = new List<AudioSource>(GetComponents<AudioSource>());
    }
}
