using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MoneyUI : MonoBehaviour
{
    public TextMeshProUGUI Text = null;
    public float AnimSpeed = 1;
    public float TipsTotalTime = .4f;
    public Timer TipsFrequency = new Timer(.2f);
    public Color TipsColor = Color.red;
    public Audio AudioMoneyInsufficient = null;

    private float currentDisplayMoney = 0;
    private int targetDisplayMoney = 0;
    private float tipsTime = 0f;

    //private IEnumerator MonyeInsufficient()
    //{
    //    var afterTime = 0f;
    //    while (afterTime < this.TipsTotalTime)
    //    {
    //        if (this.Text.color == Color.white)
    //            this.Text.color = this.TipsColor;
    //        else this.Text.color = Color.white;
    //        yield return new WaitForSeconds(this.TipsFrequency * 0.5f);
    //        afterTime += this.TipsFrequency * 0.5f;
    //    }
    //    this.Text.color = Color.white;
    //}

    /// <summary>
    /// 设置金钱
    /// </summary>
    /// <param name="value"></param>
    public void SetMoney(int value)
    {
        this.targetDisplayMoney = value;
    }
    /// <summary>
    /// 提示金钱不足，无法建造
    /// </summary>
    public void TipsMonyeInsufficientCantBuilding()
    {
        GameScene.Instance.UIManager.CreateTips(LanguageText.Hint, $"{LanguageText.CantBuilding}:", LanguageText.MonyeInsufficient);
        this.TipsFrequency.Reset();
        this.tipsTime = this.TipsTotalTime;
        this.AudioMoneyInsufficient.PlayRandomAudio();
    }
    /// <summary>
    /// 提示金钱不足，无法升级
    /// </summary>
    public void TipsMonyeInsufficientCantUpgrate()
    {
        GameScene.Instance.UIManager.CreateTips(LanguageText.Hint, $"{LanguageText.CantUpgrade}:", LanguageText.MonyeInsufficient);
        //this.TipsFrequency.Reset();
        //this.tipsTime = this.TipsTotalTime;
        this.AudioMoneyInsufficient.PlayRandomAudio();
    }

    private void Update()
    {
        if (this.currentDisplayMoney != this.targetDisplayMoney)
        {
            this.currentDisplayMoney = Mathf.Lerp(this.currentDisplayMoney, this.targetDisplayMoney, this.AnimSpeed * Time.deltaTime);
            if (Mathf.Abs(this.targetDisplayMoney - this.currentDisplayMoney) < 1)
                this.currentDisplayMoney = this.targetDisplayMoney;
            this.Text.SetText($"Money: {Mathf.CeilToInt(this.currentDisplayMoney).ToString()}");
        }

        if (this.tipsTime > 0f)
        {
            if (this.TipsFrequency.UpdateAndIsReach(Time.deltaTime))
            {
                if (this.Text.color == Color.white)
                    this.Text.color = this.TipsColor;
                else this.Text.color = Color.white;
                this.tipsTime -= this.TipsFrequency.DefiniteTime;
                if (this.tipsTime <= 0)
                    this.Text.color = Color.white;
            }
        }
    }
}
