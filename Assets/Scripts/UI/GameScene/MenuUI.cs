using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    public MenuBarUI MenuBar = null;

    public void OpenMenu()
    {
        AudioManager.Instance.Play("Click1");
        if (GameScene.Instance.IsGameOver) return;
        Time.timeScale = 0f;
        this.MenuBar.Open();
    }

    private void Update()
    {
        if (InputCtrl.IsESCKeyDown && Time.timeScale != 0f)
        {
            OpenMenu();
        }
    }
}
