using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HPUI : MonoBehaviour
{
    public Image HPBar = null;
    public Image HPAdjBar = null;
    public float AnimSpeed = 1f;

    private float targetValut = 1f;
    private float currentValut = 1f;

    public void SetHP(float hp, float hpLimit)
    {
        this.targetValut = hp / hpLimit;
        this.HPBar.fillAmount = this.targetValut;
    }

    private void Update()
    {
        this.currentValut = Mathf.Lerp(this.currentValut, this.targetValut, this.AnimSpeed * Time.deltaTime);
        this.HPAdjBar.fillAmount = this.currentValut;
    }
}
