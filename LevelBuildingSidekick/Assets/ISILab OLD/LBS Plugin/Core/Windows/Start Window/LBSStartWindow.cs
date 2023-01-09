using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using LBS.Graph;
using UnityEngine.UIElements;

using System;
using System.Reflection;
using System.IO;
using LBS.VisualElements;
using Utility;

namespace LBS.Windows
{
    public class LBSStartWindow : EditorWindow
    {
        #region Selection
        PresedBtn newLvlBtn;
        PresedBtn loadLvlBtn;
        PresedBtn infoBtn;

        VisualElement newLvlPanel;
        VisualElement loadLvlPanel;
        VisualElement infoPanel;
        #endregion

        #region NewLevel
        TextField newLvlNameField;
        Vector3Field newLvlSizeField;
        Button openNewLvlBtn;
        DropdownField lvlRepListDD;

        #endregion

        #region LoadLevel
        private List<FileInfo> jsonInfo = new List<FileInfo>();
        DropdownField loadLvlSelectionDropDown;
        Button openLoadLvlBtn;
        Button loadLoadLvlBtn; // mejorar nombre (!)
        private VisualElement contentLoad;
        #endregion

        private static List<Tuple<LBSWindowAttribute, MethodInfo>> methods = new List<Tuple<LBSWindowAttribute, MethodInfo>>();
        private static List<WindowsPreset> presets = new List<WindowsPreset>();

        [MenuItem("ISILab/LBS plugin/Welcome window...", priority = 0)]
        [MenuItem("ISILab/Welcome window", priority = 0)]
        public static void ShowWindow()
        {
            var window = GetWindow<LBSStartWindow>();
            window.titleContent = new GUIContent("Level Building Sidekick");
            //window.position = window.position;
            window.minSize = window.maxSize = new Vector2(864, 396);
            //var controller = new LBSController();

            methods = Utility.Reflection.CollectMetohdsByAttribute<LBSWindowAttribute>();
            presets = Utility.DirectoryTools.GetScriptablesByType<WindowsPreset>();
        }

        public void CreateGUI()
        {
            VisualElement root = rootVisualElement;

            var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("StartWindow");
            visualTree.CloneTree(root);

            var styleSheet = Utility.DirectoryTools.SearchAssetByName<StyleSheet>("StartWindow");
            root.styleSheets.Add(styleSheet);

            InitSelection();
            InitNewLevel();
            InitLoadLevel();
        }

        void InitSelection()
        {
            var g = rootVisualElement.Q<ButtonGroup>();
            newLvlBtn = rootVisualElement.Q<PresedBtn>(name: "NewLvlBtn");
            g.AddMember(newLvlBtn);
            loadLvlBtn = rootVisualElement.Q<PresedBtn>(name: "LoadLvlBtn");
            g.AddMember(loadLvlBtn);
            infoBtn = rootVisualElement.Q<PresedBtn>(name: "InfoBtn");
            g.AddMember(infoBtn);
            g.Init();

            newLvlPanel = rootVisualElement.Q<VisualElement>(name: "NewLevel");
            loadLvlPanel = rootVisualElement.Q<VisualElement>(name: "LoadLevel");
            infoPanel = rootVisualElement.Q<VisualElement>(name: "Info");

            OpenPanel(newLvlPanel);
            newLvlBtn.clicked += () => OpenPanel(newLvlPanel);
            loadLvlBtn.clicked += () => OpenPanel(loadLvlPanel);
            infoBtn.clicked += () => OpenPanel(infoPanel);

            //var lvls = DirectoryTools.GetAllFilesByExtension("json");
            //contentLoad

        }

        void InitNewLevel()
        {
            newLvlNameField = rootVisualElement.Q<TextField>(name: "LvlNameField");
            newLvlNameField.value = "New Level Name";

            newLvlSizeField = rootVisualElement.Q<Vector3Field>(name: "LvlSizeField");
            newLvlSizeField.value = new Vector3(64,1,64);

            //var representations = Utility.Reflection.FindDerivedTypes(typeof(LBSRepesentationData));
            //lvlRepListDD = rootVisualElement.Q<DropdownField>(name: "LvlRepListDD");
            //lvlRepListDD.choices = representations.Select(r => r.ToString()).ToList();
            
            openNewLvlBtn = rootVisualElement.Q<Button>(name: "OpenNewLvlBtn"); 
            openNewLvlBtn.clicked += OpenNewLevel;

            //loadLoadLvlBtn = rootVisualElement.Q<Button>(name: "LoadLoadLvlBtn");
            //loadLoadLvlBtn.clicked += LoadFromFolder;

            contentLoad = rootVisualElement.Q<VisualElement>("ContentLoad");

        }

        private void LoadFromFolder()
        {

        }

        void InitLoadLevel()
        {
            loadLvlSelectionDropDown = rootVisualElement.Q<DropdownField>(name: "LoadLvlSelectionDD");
            openLoadLvlBtn = rootVisualElement.Q<Button>(name: "OpenLoadLvlBtn");

            var path = Application.dataPath;
            jsonInfo = Utility.DirectoryTools.GetAllFilesByExtension(".json",path); // esto encuentra todos los json, incluso lo que no son lvls (!!)
            //jsonPaths = Utility.JSONDataManager.GetJSONFiles(Application.dataPath + "/LBSLevels"); //deberia traerlo de cualquier ruta en el proyecto o no?
            loadLvlSelectionDropDown.choices = jsonInfo.Select(fi => fi.Name).ToList();

            openLoadLvlBtn.clicked += LoadLevel;

        }

        void InitInfo()
        {

        }

        private void OpenPanel(VisualElement element) // hay mejores formas de hacer esto (!)
        {
            newLvlPanel.visible = (newLvlPanel == element);
            loadLvlPanel.visible = (loadLvlPanel == element);
            infoPanel.visible = (infoPanel == element);
        }

        private void OpenPresetWindow() // mejor nombre de metodo (!)
        {
            var firstPreset = presets[0]; // esto es temporal (!)
            foreach (var wName in firstPreset.Windows)
            {
                var m = methods.Find(t => t.Item1.Name == wName);
                try
                {
                    var action = (Action)m.Item2.CreateDelegate(typeof(Action));
                    action.Invoke();
                }
                catch { }
            }
        }

        void OpenNewLevel()
        {
            var name = newLvlNameField.value;
            var size = newLvlSizeField.value;
            LBSController.CreateNewLevel(name, size);
            OpenPresetWindow();
            this.Close();
        }

        void LoadLevel()
        {
            var index = loadLvlSelectionDropDown.index;
            var selected = jsonInfo[index];
            if (selected != null)
            {
                LBSController.LoadFile(selected.FullName);
                OpenPresetWindow();
                this.Close();
            }
            else
            {
                Debug.LogWarning("Select a level file to open.");
            }
            
        }
    }
}
