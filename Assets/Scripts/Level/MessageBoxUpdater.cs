using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageBoxUpdater : MonoBehaviour
{
    private IMessageBox messageBox = null;
    private static TextBox currentTextBox = null;

    public Camera GetCamera => Camera.main;

    public static void ResetCurrentTextBox()
    {
        if (currentTextBox != null && !currentTextBox.IsReleased)
        {
            currentTextBox.ReleaseThis();
        }
        currentTextBox = null;
    }

    private void Update()
    {
        //if (Mathf.Approximately(Time.timeScale, 0f))
        //    return;
        IMessageBox mb = null;

        var hits2D = Physics2D.GetRayIntersectionAll(GameScene.Instance.UIManager.UiCamera.ScreenPointToRay(InputCtrl.MousePosition), float.MaxValue, UIManager.UI_LAYER_MASK);
        for (var i = 0; i < hits2D.Length; i++)
        {
            if (hits2D[i].transform != null)
            {
                mb = hits2D[i].transform.GetComponent<IMessageBox>();
                if (mb != null) break;
            }
        }
        if (mb == null)
        {
            var hits = Physics.RaycastAll(InputCtrl.MainMouseRay, float.MaxValue, LayerMask.GetMask("Building"));
            for (var i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform != null)
                {
                    mb = hits[i].transform.root.GetComponent<IMessageBox>();
                    break;
                }
            }
        }

        if (mb != null)
        {
            if (this.messageBox != mb)
            {
                ResetCurrentTextBox();
                this.messageBox = mb;
                currentTextBox = UIManager.TextBoxPoolObject.Take();
                StartCoroutine(currentTextBox.AfterTakedInitialize(mb.GetMessageText));
            }
        }
        else
        {
            this.messageBox = null;
            ResetCurrentTextBox();
        }
    }
}

/// <summary>
/// 当鼠标射线命中时返回文字信息
/// </summary>
public interface IMessageBox
{
    string GetMessageText { get; }
}
