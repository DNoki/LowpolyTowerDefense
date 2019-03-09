using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.PostProcessing;

public class LoadScene : MonoBehaviour
{
    public Setting SettingWindow = null;
    public Producer ProducerWindow = null;
    public Loading Loading = null;
    public List<RectTransform> Items = null;
    public PostProcessProfile PostProcessProfile = null;

    public DepthOfField DepthOfField { get; set; }

    public IEnumerator Load(string name)
    {
        Close();
        this.Loading.gameObject.SetActive(true);
        yield return new WaitForSecondsRealtime(1f);

        var operation = SceneManager.LoadSceneAsync(name, LoadSceneMode.Single);

        var timer = new Timer(10f);
        var isTimerOver = false;

        while (!operation.isDone || !isTimerOver)
        {
            this.Loading.Progress = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
            if (timer.UpdateAndIsReach(Time.deltaTime))
                isTimerOver = true;
        }
    }

    public void StartNormal1()
    {
        AudioManager.Instance.Play("Click1");
        StartCoroutine(Load("Normal1"));
    }
    public void StartFreeMode()
    {
        AudioManager.Instance.Play("Click1");
        StartCoroutine(Load("FreeMode"));
    }
    public void Setting()
    {
        AudioManager.Instance.Play("Click1");
        this.SettingWindow.OnClose -= Open;
        this.SettingWindow.OnClose += Open;
        this.SettingWindow.Open();
        this.Close();
    }
    public void Producer()
    {
        AudioManager.Instance.Play("Click1");
        this.ProducerWindow.OnClose -= Open;
        this.ProducerWindow.OnClose += Open;
        this.ProducerWindow.Open();
        this.Close();
    }
    public void Exit()
    {
        AudioManager.Instance.Play("Click1");
        Application.Quit();
    }

    public void Open()
    {
        this.DepthOfField.focalLength.value = 50f;
        this.SettingWindow.OnClose -= Open;
        foreach (var item in this.Items)
            item.gameObject.SetActive(true);
    }
    public void Close()
    {
        this.DepthOfField.focalLength.value = 1f;
        foreach (var item in this.Items)
            item.gameObject.SetActive(false);
    }

    private void Awake()
    {
        if(this.DepthOfField==null)
        this.DepthOfField = this.PostProcessProfile.GetSetting<DepthOfField>();
        this.DepthOfField.focalLength.value = 50f;
    }
}
