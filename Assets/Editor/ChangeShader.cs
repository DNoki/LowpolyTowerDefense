using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ChangeShader : EditorWindow
{
    public Shader LWRP_Building = null;

    [MenuItem("TD/ChangeShader")]
    public static void ShowWindow()
    {
        GetWindow<ChangeShader>("ChangeShader");
    }

    private void OnGUI()
    {
        if (this.LWRP_Building == null)
            this.LWRP_Building = Resources.Load<Shader>("Shaders/LWRP_Building");

        GUILayout.Label("选择材质", EditorStyles.boldLabel);
        GUILayout.TextField($"数量{Selection.objects.Length}");

        if (GUILayout.Button("修改Shader"))
        {
            foreach (var obj in Selection.objects)
            {
                if (obj is Material)
                {
                    var material = obj as Material;
                    var color = material.color;
                    if (this.LWRP_Building == null)
                    {
                        Debug.Log("LWRP_Building 未找到！");
                        return;
                    }
                    material.shader = this.LWRP_Building;
                    material.SetColor("_MainColor", color);
                }
            }
        }
    }

    private void OnSelectionChange()
    {
        Repaint();
    }
}
