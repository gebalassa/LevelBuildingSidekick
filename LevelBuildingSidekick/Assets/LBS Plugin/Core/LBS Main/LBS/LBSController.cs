using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using LevelBuildingSidekick.Graph;

namespace LevelBuildingSidekick
{
    public class LBSController
    {
        #region InspectorDrawer
        private class LevelScriptable : GenericScriptable<LevelData> { };
        [CustomEditor(typeof(LevelScriptable))]
        [CanEditMultipleObjects]
        private class LevelScriptableEditor : GenericScriptableEditor { };
        #endregion

        private static LevelBackUp backUp;

        public static LevelData CurrentLevel
        {
            get
            {
                LoadBackup();
                return backUp.level;
            }
            set
            {
                LoadBackup();
                backUp.level = value;
            }
        }

        [MenuItem("LBS/[Old] Welcome window...",priority = 0)]
        public static void ShowWindow()
        {
            var window = LBSStartWindow.GetWindow<LBSStartWindow>("Level Building Sidekick");
        }

        private static void LoadBackup()
        {
            if (backUp == null)
            {
                backUp = Resources.Load("LBSBackUp") as LevelBackUp;
                if (backUp == null)
                {
                    backUp = ScriptableObject.CreateInstance<LevelBackUp>();
                    if(!Directory.Exists("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources")) // esto podria ser peligroso (!)
                    {
                        Directory.CreateDirectory("Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources"); // esto podria ser peligroso (!)
                    }
                    AssetDatabase.CreateAsset(backUp, "Assets/LevelBuildingSidekick/Core/LBS Main/Level/Resources/LBSBackUp.asset"); // esto podria ser peligroso (!)
                    AssetDatabase.SaveAssets();
                }
            }
        }

        public static LevelData CreateLevel(string levelName, Vector3 size)
        {
            LevelData data = new LevelData();
            data.levelName = levelName;
            data.Size = size;
            data.representations.Add(new LBSGraphData());
            return data;
        }

        public static void ShowLevelInspector()
        {
            var s = ScriptableObject.CreateInstance<LevelScriptable>();
            s.data = CurrentLevel;
            Selection.SetActiveObjectWithContext(s, s);
        }
    }
}


