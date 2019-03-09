using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Producer : MonoBehaviour
{
    public event Action OnClose = null;

    public void Open()
    {
        this.gameObject.SetActive(true);
    }
    public void Close()
    {
        AudioManager.Instance.Play("Click2");
        OnClose?.Invoke();
        this.gameObject.SetActive(false);
    }
}
