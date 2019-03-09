using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageText : MonoBehaviour, IMessageBox
{
    [TextArea]
    public string Text = string.Empty;

    string IMessageBox.GetMessageText => this.Text;
}
