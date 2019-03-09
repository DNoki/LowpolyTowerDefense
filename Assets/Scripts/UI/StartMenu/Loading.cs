using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Loading : MonoBehaviour
{
    public RectTransform RectTransform = null;
    public Slider Bar = null;

    public float Progress
    {
        get { return this.Bar.value; }
        set { this.Bar.value = value; }
    }
}
