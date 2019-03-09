using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TipsUI : MonoBehaviour, IPoolObject
{
    [SerializeField] private RectTransform rectTransform = null;
    public Animator Anim = null;
    public Image DescriptionBG = null;
    public TextMeshProUGUI LableTMP = null;
    public TextMeshProUGUI TitleTMP = null;
    public TextMeshProUGUI TextTMP = null;
    /// <summary>
    /// 自动退出时间
    /// </summary>
    public Timer AutoExitTimer = new Timer(10f);

    /// <summary>
    /// 初始位置
    /// </summary>
    [Space(20)] public Vector2 Position = new Vector2(-824f, 358f);
    /// <summary>
    /// 次位置偏移
    /// </summary>
    public float PositionOffsetY = 0f;
    /// <summary>
    /// 最小位置下限
    /// </summary>
    public float PositionMinLimitY = 0f;
    /// <summary>
    /// 说明框高度偏移
    /// </summary>
    public float DescriptionBGOffsetY = 38f;
    /// <summary>
    /// 说明文本位置偏移
    /// </summary>
    public float TextOffsetY = -32f;

    public bool IsReleased { get; set; }
    public RectTransform RectTransform => this.rectTransform;
    public float HeightEndPosition => this.RectTransform.anchoredPosition.y - this.TitleTMP.preferredHeight - this.TextTMP.preferredHeight;

    void IPoolObject.Initialize()
    {
        this.AutoExitTimer.Reset();
        this.IsReleased = false;
        this.gameObject.SetActive(true);
        this.Anim.SetBool("IsExit", false);
    }
    void IPoolObject.Release()
    {
        this.IsReleased = true;
        this.gameObject.SetActive(false);
    }

    private IEnumerator ReleaseThis()
    {
        this.IsReleased = true;
        this.Anim.SetBool("IsExit", true);
        var color = new Color(0f, 0f, 0f, 0.3f);
        this.DescriptionBG.color = color;

        var tipsPool = UIManager.TipsPoolObject;
        tipsPool.RemoveTips(this);
        while (true)
        {
            var state = this.Anim.GetCurrentAnimatorStateInfo(0);
            if (state.IsName("TipsExitAnim") && state.normalizedTime >= 1)
                break;
            yield return null;
        }
        tipsPool.ReleasePut(this);
    }
    /// <summary>
    /// 设置文本并自动校正位置
    /// </summary>
    /// <param name="lable">标签</param>
    /// <param name="title">标题</param>
    /// <param name="text">内容</param>
    /// <returns></returns>
    public TipsUI SetText(string lable, string title, string text)
    {
        this.LableTMP.SetText(lable);
        this.TitleTMP.SetText(title);
        this.TextTMP.SetText(text);

        // 设置背景黑框高度
        var size = this.DescriptionBG.rectTransform.sizeDelta;
        size.y = this.DescriptionBGOffsetY + this.TitleTMP.preferredHeight + this.TextTMP.preferredHeight;
        this.DescriptionBG.rectTransform.sizeDelta = size;

        // 设置文本位置
        var pos = this.TextTMP.rectTransform.anchoredPosition;
        pos.y = this.TextOffsetY - this.TitleTMP.preferredHeight;
        this.TextTMP.rectTransform.anchoredPosition = pos;
        return this;
    }

    public void Selected()
    {
        if (this.IsReleased) return;
        var color = this.DescriptionBG.color;
        color.a = 0.8f;
        this.DescriptionBG.color = color;
    }
    public void UnSelected()
    {
        if (this.IsReleased) return;
        var color = new Color(0f, 0f, 0f, 0.3f);
        this.DescriptionBG.color = color;
    }
    public void MouseDown()
    {
        if (this.IsReleased) return;
        var color = new Color(.2f, .2f, .2f, 0.9f);
        this.DescriptionBG.color = color;
    }
    public void MouseClick()
    {        
        StartCoroutine(ReleaseThis());
    }

    private void Update()
    {
        if (this.IsReleased) return;
        if (this.AutoExitTimer.UpdateAndIsReach(Time.deltaTime))
        {
            StartCoroutine(ReleaseThis());
        }
    }
}

