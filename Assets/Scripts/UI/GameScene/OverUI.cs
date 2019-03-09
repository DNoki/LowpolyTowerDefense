using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class OverUI : MonoBehaviour
{
    public Animator Anim = null;
    public Button GoHome = null;
    public Button Replay = null;
    public string StateName = string.Empty;

    public void DoGoHome()
    {
        AudioManager.Instance.Play("Click1");
        Time.timeScale = 1f;
        SceneManager.LoadScene("StartMenu", LoadSceneMode.Single);
    }
    public void DoReplay()
    {
        AudioManager.Instance.Play("Click1");
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

    public void Play()
    {
        this.gameObject.SetActive(true);
        GameScene.Instance.UIManager.BuildingList.gameObject.SetActive(false);
        this.Anim.Play(this.StateName);
        StartCoroutine(DisplayButton(this.Anim.GetCurrentAnimatorStateInfo(0).length));
    }

    public IEnumerator DisplayButton(float delay)
    {
        yield return new WaitForSeconds(delay);
        this.GoHome.gameObject.SetActive(true);
        this.Replay.gameObject.SetActive(true);
    }
}
