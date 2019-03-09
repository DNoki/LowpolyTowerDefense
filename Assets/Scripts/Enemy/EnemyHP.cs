using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class EnemyHP : MonoBehaviour
{
    public Image Bar = null;

    public Transform GetCameraTransform => Camera.main.transform;

    void Update()
    {
        this.transform.rotation = Quaternion.LookRotation(this.GetCameraTransform.forward, this.GetCameraTransform.up);
        //this.transform.LookAt(this.transform.position - this.GetCamera.transform.position);
    }
}
