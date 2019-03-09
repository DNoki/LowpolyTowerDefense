//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEditor.ShaderGraph;
//using System.Reflection;

//[Title("Custom", "Gaussian Blur")]
//public class ShaderGraphFunction : CodeFunctionNode
//{
//    protected override MethodInfo GetFunctionToConvert()
//    {
//        return GetType().GetMethod("GaussianBlurFunction", BindingFlags.Static | BindingFlags.NonPublic);
//    }

//    static string GaussianBlurFunction(
//        [Slot(0, Binding.None)]DynamicDimensionVector A,
//        [Slot(1, Binding.None)]DynamicDimensionVector B,
//        [Slot(2, Binding.None)]Texture2D Texture,
//        [Slot(3, Binding.None)]out DynamicDimensionVector Out)
//    {
//        return
//@"
//    {
//        Out = A + B;
//    }
//";
//    }

//    public ShaderGraphFunction()
//    {
//        this.name = "Gaussian Blur";
//    }
//}
