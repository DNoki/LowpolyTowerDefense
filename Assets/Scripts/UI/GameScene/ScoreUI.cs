using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScoreUI : MonoBehaviour
{
    public TextMeshProUGUI Text = null;
    public float AnimSpeed = 1;

    private float currentDisplayScore = 0;
    private int targetDisplayScore = 0;

    public void SetScore(int value)
    {
        this.targetDisplayScore = value;
    }

    private void Update()
    {
        if (this.currentDisplayScore == this.targetDisplayScore)
            return;
        this.currentDisplayScore = Mathf.Lerp(this.currentDisplayScore, this.targetDisplayScore, this.AnimSpeed * Time.deltaTime);
        if (this.targetDisplayScore - this.currentDisplayScore < 1)
            this.currentDisplayScore = this.targetDisplayScore;
        this.Text.SetText($"Score: {Mathf.CeilToInt(this.currentDisplayScore).ToString()}");
    }
}
