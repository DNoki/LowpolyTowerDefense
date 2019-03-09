using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProcedureColorSpace : MonoBehaviour
{
    public Renderer Renderer = null;

    private Texture2D tex = null;

    private void Start()
    {
        var width = 1024;
        var height = 1024;
        this.tex = new Texture2D(width, height);
        this.Renderer.material.mainTexture = this.tex;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                var h = ((float)y / height);
                var xF = (float)x / width;
                var s = FunctionExtension.Remap(xF, 0.5f, 1f, 1, 0f);
                var v = FunctionExtension.Remap(xF, 0.0f, 0.5f, 0f, 1f);

                if (xF < 0.5f) s = 1f;
                else if (xF > 0.5f) v = 1f;
                else s = v = 1f;

                var color = Color.HSVToRGB(h, s, v);
                this.tex.SetPixel(x, y, color);
            }
        }
        this.tex.Apply();
        System.IO.File.WriteAllBytes($"{Application.dataPath}/ColorSpace", this.tex.EncodeToPNG());
    }
}