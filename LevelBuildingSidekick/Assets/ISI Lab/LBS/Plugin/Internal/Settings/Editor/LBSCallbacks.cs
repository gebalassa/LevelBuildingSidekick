using LBS.Bundles;
using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ISILab.LBS.Internal.Editor
{
    [InitializeOnLoad]
    public class LBSCallbacks
    {
        static LBSCallbacks()
        {
            var onStart = SessionState.GetBool("start", true);
            if (onStart)
            {
                EditorApplication.update += OnStartEditor;
                SessionState.SetBool("start", false);
            }

            AssemblyReloadEvents.afterAssemblyReload += OnAfterReloadScript;
            AssemblyReloadEvents.beforeAssemblyReload += OnBeforeReloadScript;
        }

        /// <summary>
        /// called when the editor starts for the first time
        /// </summary>
        private static void OnStartEditor()
        {
            SettingsEditor.SearchSettingsInstance();
            ReloadBundles();

            EditorApplication.update -= OnStartEditor;
        }

        /// <summary>
        /// called before the script is reloaded
        /// </summary>
        private static void OnBeforeReloadScript()
        {
            SaveBackUp();
        }

        /// <summary>
        /// called after the script is reloaded
        /// </summary>
        private static void OnAfterReloadScript()
        {
            LoadBackUp();
            ReloadCurrentLevel();
        }

        /// <summary>
        /// save the level in the backup temporarily
        /// </summary>
        private static void SaveBackUp()
        {
            var level = LBS.loadedLevel;
            if (level != null)
            {
                var settings = LBSSettings.Instance;
                var path = settings.paths.backUpPath;
                var folderPath = Path.GetDirectoryName(path);

                var backUp = ScriptableObject.CreateInstance<BackUp>();
                backUp.level = level;

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                AssetDatabase.CreateAsset(backUp, path);
                AssetDatabase.SaveAssets();
            }
            else
            {
                Debug.LogError("[ISI Lab]: Error on save BackUp");
            }
        }

        /// <summary>
        /// Load the level from the backup and delete it
        /// </summary>
        private static void LoadBackUp()
        {
            // search and set the instance of "LBS Settings" in its singleton
            var settings = LBSSettings.Instance;
            var path = settings.paths.backUpPath;
            var backUp = AssetDatabase.LoadAssetAtPath<BackUp>(path);

            if (backUp != null)
            {
                // load the level from the backup
                LBS.loadedLevel = backUp.level;

                // After loading the level, the backup is destroyed
                AssetDatabase.DeleteAsset(path);
            }
            else
            {
                // if the backup is not found, a new level is created
                LBS.loadedLevel = LoadedLevel.CreateInstance(new LBSLevelData(), "New level");
            }
        }

        public static void ReloadStorage()
        {
            var storage = LBSAssetsStorage.Instance;
            storage.SearchInProject();
        }

        public static void ReloadCurrentLevel()
        {
            var data = LBS.loadedLevel.data;
            data?.Reload();
        }

        public static void ReloadBundles()
        {
            var storage = LBSAssetsStorage.Instance;
            var bundles = storage.Get<Bundle>();
            foreach (var bundle in bundles)
            {
                bundle.Reload();
            }
        }

    }
}