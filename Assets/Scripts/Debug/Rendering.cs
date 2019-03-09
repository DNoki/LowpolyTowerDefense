using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Rendering : MonoBehaviour
{
    public bool Enable = false;
    public LoadScene LoadUI = null;
    public int Seed = 0;


    private IEnumerator Render()
    {
        var size = new Vector2Int(Screen.width, Screen.height);
        var snapshot = new Texture2D(size.x, size.y, TextureFormat.ARGB32, false);

        yield return new WaitForEndOfFrame();

        snapshot.ReadPixels(new Rect(0, 0, size.x, size.y), 0, 0, false);
        snapshot.Apply();
        var data = snapshot.EncodeToPNG();
        File.WriteAllBytes($"{Application.dataPath}/Snapshots~/snap_{Time.frameCount}.png", data);
        //File.WriteAllBytes($"{Application.dataPath}/Snapshots/snap_{System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png", data);
    }

    private void Awake()
    {
        this.Seed = System.DateTime.Now.GetHashCode();
    }
    private void Start()
    {
        //if (this.LoadUI.DepthOfField == null)
        //    this.LoadUI.DepthOfField = this.LoadUI.PostProcessProfile.GetSetting<DepthOfField>();
        //this.LoadUI.DepthOfField.focalLength.value = 1f;        
        Directory.CreateDirectory($"{Application.dataPath}/Snapshots~");
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F6))
        {
            this.Enable = !this.Enable;
        }
        if (this.Enable)
        {
            if (Application.targetFrameRate != 60) Application.targetFrameRate = 60;
            if (Time.captureFramerate != 60) Time.captureFramerate = 60;
            if (Time.frameCount % 2 == 0)
            {
                //StartCoroutine(Render());
                ScreenCapture.CaptureScreenshot($"{Application.dataPath}/Snapshots~/snap_{this.Seed}_{Time.frameCount}.png");
            }
        }
        else
        {
            Time.captureFramerate = 0;
        }

    }
    private void OnApplicationQuit()
    {
        //this.LoadUI.DepthOfField.focalLength.value = 1f;
    }
}
