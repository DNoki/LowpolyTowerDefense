using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRotate : MonoBehaviour
{
    public Transform[] Crystal = null;
    public float RotateSpeed = 30f;

    private void Update()
    {
        for (var i = 0; i < this.Crystal.Length; i++)
        {
            this.Crystal[i].Rotate(Vector3.up, this.RotateSpeed * Time.deltaTime);
        }
    }
}
