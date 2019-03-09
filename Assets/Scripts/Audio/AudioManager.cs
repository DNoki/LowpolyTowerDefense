using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public List<IndependentAudio> Audios = null;

    private Dictionary<string, int> audioQualityDic = new Dictionary<string, int>();

    public static AudioManager Instance { get; private set; } = null;

    /// <summary>
    /// 名前指定された音を流す
    /// </summary>
    /// <param name="name"></param>
    public void Play(string name)
    {
        var audio = this.Audios.Find(a => name == a.Name);
        if (audio == null)
        {
            Debug.LogWarning("未找到指定名称的音频。");
            return;
        }
        audio.Source.Play();
    }
    /// <summary>
    /// 名前指定された音リストからランダムに選んで流す
    /// </summary>
    /// <param name="names"></param>
    public void PlayRandomAudio(params string[] names)
    {
        if (names.Length <= 0) return;
        var name = names[Random.Range(0, names.Length)];
        Play(name);
    }
    public void PlayInstance(string name)
    {
        var value = 0;
        if(this.audioQualityDic.TryGetValue(name, out value))
        {
            if (value > 3) return;
        }
        else this.audioQualityDic.Add(name, 0);
        //if (this.audioQualityDic.ContainsKey(name))
        //{
        //    if (this.audioQualityDic[name] > 3)
        //        return;
        //}
        //else this.audioQualityDic.Add(name, 0);
        this.audioQualityDic[name]++;

        var audioPrefab = this.Audios.Find(a => name == a.Name);
        if (audioPrefab == null)
        {
            Debug.LogWarning("未找到指定名称的音频。");
            return;
        }
        var audio = new IndependentAudio();
        audio.Name = name;
        audio.Source = this.gameObject.AddComponent<AudioSource>();
        audio.Source.clip = audioPrefab.Clip;
        audio.Source.outputAudioMixerGroup = audioPrefab.AMG;
        audio.Source.pitch = audioPrefab.Pitch;
        audio.Source.volume = audioPrefab.Volume;
        audio.Source.loop = audioPrefab.IsLoop;
        audio.Source.Play();
        StartCoroutine(PlayInstanceImplement(audio));
    }
    private IEnumerator PlayInstanceImplement(IndependentAudio audio)
    {
        yield return new WaitForSecondsRealtime(audio.Source.clip.length);
        this.audioQualityDic[audio.Name]--;
        Destroy(audio.Source);
    }

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(this.gameObject);
            return;
        }
        DontDestroyOnLoad(this.gameObject);

        if (this.audioQualityDic == null) this.audioQualityDic = new Dictionary<string, int>();

        foreach (var audio in this.Audios)
        {
            audio.Source = this.gameObject.AddComponent<AudioSource>();
            audio.Source.clip = audio.Clip;
            audio.Source.outputAudioMixerGroup = audio.AMG;
            audio.Source.pitch = audio.Pitch;
            audio.Source.volume = audio.Volume;
            audio.Source.loop = audio.IsLoop;
        }

        Play("BGM1");
    }
}