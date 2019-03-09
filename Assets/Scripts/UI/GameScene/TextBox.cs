using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextBox : MonoBehaviour, IPoolObject
{
    public RectTransform RectTransform = null;
    public Image Image = null;
    public TextMeshProUGUI Text = null;
    /// <summary>
    /// 位置偏移
    /// </summary>
    public Vector2 Offset = Vector2.zero;
    public Vector2 SizeOffset = Vector2.zero;

    public bool IsReleased { get; private set; }

    void IPoolObject.Initialize()
    {
        this.transform.SetParent(GameScene.Instance.UIManager.transform, false);
        this.gameObject.SetActive(true);
    }
    void IPoolObject.Release()
    {
        this.gameObject.SetActive(false);
    }
    public IEnumerator AfterTakedInitialize(string text)
    {
        this.Text.SetText(text);
        yield return null;

        var textOffset = new Vector2(Mathf.Abs(this.Text.rectTransform.anchoredPosition.x), Mathf.Abs(this.Text.rectTransform.anchoredPosition.y));
        this.Image.rectTransform.sizeDelta = this.Text.bounds.size.xy() + textOffset + this.SizeOffset;
    }
    public void ReleaseThis()
    {
        UIManager.TextBoxPoolObject.Release(this);
    }

    private void Update()
    {
        var textOffset = new Vector2(Mathf.Abs(this.Text.rectTransform.anchoredPosition.x), Mathf.Abs(this.Text.rectTransform.anchoredPosition.y));
        this.Image.rectTransform.sizeDelta = this.Text.bounds.size.xy() + textOffset + this.SizeOffset;

        var pos = InputCtrl.MousePosition;
        pos = new Vector2(FunctionExtension.Remap(pos.x, 0f, Screen.width, 0f, 1920f), FunctionExtension.Remap(pos.y, 0f, Screen.height, 0f, 1080f));
        this.RectTransform.anchoredPosition = pos + this.Offset + Vector2.up * this.Image.rectTransform.sizeDelta.y;

    }
}

