using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using System.IO;
using LevelBuildingSidekick.Graph;
using System.Text;

namespace LevelBuildingSidekick
{
    // change name "LBSController" to "LBS" or "LBSCore" or "LBSMain" (!)
    // esta clase podria ser estatica completamente (??)
    public class LBSController 
    {
        #region InspectorDrawer
        private class LevelScriptable : GenericScriptable<LevelData> { };
        [CustomEditor(typeof(LevelScriptable)),CanEditMultipleObjects]
        private class LevelScriptableEditor : GenericScriptableEditor { };
        #endregion

        //private static LevelBackUp backUp;

        public static LevelData CurrentLevel
        {
            get
            {
                var instance = LevelBackUp.Instance();
                return instance.level;
                //LoadBackup();
                //return backUp.level;
            }
            set
            {
                var instance = LevelBackUp.Instance();
                instance.level = value;
                //LoadBackup();
                //backUp.level = value;
            }
        }


        internal static void LoadFile()
        {
            var answer = EditorUtility.DisplayDialogComplex(
                   "The current file has not been saved",
                   "if you open a file the progress in the current document will be lost, are you sure to continue?",
                   "save",
                   "discard",
                   "cancel");
            string path;
            switch(answer)
            {
                case 1:
                    SaveFile();
                    path = EditorUtility.OpenFilePanel("Load level data", "", ".json");
                    CurrentLevel = Utility.JSONDataManager.LoadData<LevelData>(path);
                    break;
                case 2:
                    path = EditorUtility.OpenFilePanel("Load level data", "", ".json");
                    CurrentLevel = Utility.JSONDataManager.LoadData<LevelData>(path);
                    break;
                case 3:
                    // do nothing
                    break;
                default:
                    // do nothing
                    break;
            }
        }

        public static bool FileExists(string name, string extension, out FileInfo toReturn)
        {
            var path = Application.dataPath;
            System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(path);
            var files = Utility.DirectoryTools.GetAllFilesByExtension(extension, dir);

            var fileInfo = files.Find(f => f.Name.Contains(name));

            toReturn = fileInfo;
            return fileInfo != null;
        }

        internal static void SaveFile()
        {
            if(CurrentLevel.levelName == "")
                SaveFileAs();

            FileInfo fileInfo;
            if (FileExists(CurrentLevel.levelName, ".json", out fileInfo))
            {
                Utility.JSONDataManager.SaveData(fileInfo.FullName, LBSController.CurrentLevel);
            }
            else
            {
                SaveFileAs();
            }

        }

        internal static void SaveFileAs()
        {
            var lvl = CurrentLevel;

            var name = lvl.levelName;
            var path = EditorUtility.SaveFilePanel("Save level data", "", name + ".json", "json");

            if (path != "")
            {
                Debug.Log("Save file on: '" + path + "'.");
                Utility.JSONDataManager.SaveData(path, LBSController.CurrentLevel);
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


