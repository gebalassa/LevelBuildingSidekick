﻿using UnityEditor;
using UnityEngine;

namespace Assets.ISI_Lab.LBS.Artificial_Intelligence.Optimization.HillClimbingEvaluator
{
    public class NewEditorScript1 : ScriptableObject
    {
        [MenuItem("Tools/MyTool/Do It in C#")]
        static void DoIt()
        {
            EditorUtility.DisplayDialog("MyTool", "Do It in C# !", "OK", "");
        }
    }
}