using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuBarUI : MonoBehaviour
{
    public MenuUI Menu = null;
    public Setting SettingWindow = null;

    public void GoHome()
    {
        AudioManager.Instance.Play("Click1");
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }
    public void Setting()
    {
        AudioManager.Instance.Play("Click1");
        this.SettingWindow.OnClose -= Open;
        this.SettingWindow.OnClose += Open;
        this.SettingWindow.Open();
        this.gameObject.SetActive(false);
    }
    public void Back()
    {
        AudioManager.Instance.Play("Click1");
        Time.timeScale = 1f;
        this.gameObject.SetActive(false);
    }

    public void Open()
    {        
        this.SettingWindow.OnClose -= Open;
        this.gameObject.SetActive(true);
    }
}
