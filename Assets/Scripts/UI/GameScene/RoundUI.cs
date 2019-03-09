using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RoundUI : MonoBehaviour
{
    public TextMeshProUGUI RoundValue = null;
    public TextMeshProUGUI RoundLable = null;

    public void SetRound(int current, int total)
    {
        this.RoundValue.SetText(current.ToString());
        if (current < 10)
            this.RoundLable.SetText($"/{total.ToString()}");
        else this.RoundLable.SetText($"   /{total.ToString()}");
    }
}
