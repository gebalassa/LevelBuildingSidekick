using LBS;
using LBS.Components;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using Utility;

public class LBSMainWindow : EditorWindow
{
    // Data
    private LBSLevelData levelData;

    // Selected
    private LBSLayer _selectedLayer;

    // Templates
    public List<LayerTemplate> layerTemplates;

    // Visual Elements
    private ButtonGroup toolPanel;
    private VisualElement extraPanel;
    private VisualElement noLayerSign;
    private MainView mainView; // work canvas
    private Label selectedLabel;
    private VisualElement floatingPanelContent;

    // Panels
    private LayersPanel layerPanel;
    private AIPanel aiPanel;
    private Generator3DPanel gen3DPanel;
    private LayerInspector layerInspector;

    // Manager
    //private ToolkitManager toolkitManager;
    private DrawManager drawManager;
    private LBSInspectorPanel inspectorManager;


    [MenuItem("ISILab/Level Building Sidekick", priority = 0)]
    private static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/LBS_Logo1");
        window.titleContent = new GUIContent("Level builder", icon);
        window.minSize = new Vector2(800, 400);
    }

    private static LBSMainWindow _ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/LBS_Logo1");
        window.titleContent = new GUIContent("Level builder", icon);
        return window;
    }

    public virtual void CreateGUI()
    {
        Init();
    } 

    private void Init()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        // LayerTemplate
        layerTemplates = Utility.DirectoryTools.GetScriptablesByType<LayerTemplate>();

        // SubPanelScrollView
        var subPanelScrollView = rootVisualElement.Q<ScrollView>("SubPanelScrollView");
        subPanelScrollView.Q<VisualElement>("unity-content-and-vertical-scroll-container").pickingMode = PickingMode.Ignore;
        subPanelScrollView.Q<VisualElement>("unity-content-viewport").pickingMode = PickingMode.Ignore;
        subPanelScrollView.Q<VisualElement>("unity-content-container").pickingMode = PickingMode.Ignore;

        // ToolPanel
        toolPanel = rootVisualElement.Q<ButtonGroup>("ToolsGroup");

        // LayerInspector
        layerInspector = rootVisualElement.Q<LayerInspector>("LayerInspector");

        // MainView 
        mainView = rootVisualElement.Q<MainView>("MainView");
        mainView.OnClearSelection = () =>
        {
            if (_selectedLayer != null)
            {
                var il = Reflection.MakeGenericScriptable(_selectedLayer);
                Selection.SetActiveObjectWithContext(il, il);
            }
        };

        // DrawManager
        drawManager = new DrawManager(ref mainView, ref layerTemplates);

        // InspectorContent
        inspectorManager = rootVisualElement.Q<LBSInspectorPanel>("InpectorPanel");

        // ToolKitManager
        /*
        toolkitManager = new ToolkitManager(ref toolPanel, ref mainView, ref inspectorManager, ref layerTemplates);
        toolkitManager.OnEndSomeAction += () =>
        {
            drawManager.Redraw(levelData, mainView);
        };
        */

        // ToolBar
        var toolbar = rootVisualElement.Q<ToolBarMain>("ToolBar");
        toolbar.OnNewLevel += (data) =>
        {
            LevelBackUp.Instance().level = data;
            levelData = LevelBackUp.Instance().level.data;
            RefreshWindow();
        };
        toolbar.OnLoadLevel += (data) =>
        {
            LevelBackUp.Instance().level = data;
            levelData = LevelBackUp.Instance().level.data;
            RefreshWindow();
            drawManager.Redraw(levelData, mainView);
        };

        // ExtraPanel
        extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

        // NoLayerSign
        noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");

        // SelectedLabel
        selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");


        // FloatingPanelContent
        floatingPanelContent = rootVisualElement.Q<VisualElement>("FloatingPanelContent");

        // Init Data
        levelData = LBSController.CurrentLevel.data;
        OnLevelDataChange(levelData);
        levelData.OnChanged += (lvl) => {
            OnLevelDataChange(lvl);
        };

        // LayerPanel
        layerPanel = new LayersPanel(ref levelData, ref layerTemplates);
        extraPanel.Add(layerPanel);
        layerPanel.style.display = DisplayStyle.Flex;
        layerPanel.OnLayerVisibilityChange += (l) =>
        {
            drawManager.Redraw(levelData, mainView);
        };
        layerPanel.OnSelectLayer += ShowinfoLayer;
        layerPanel.OnAddLayer += ShowinfoLayer;

        // AIPanel
        aiPanel = new AIPanel();
        aiPanel.OnFinish += () =>
        {
            drawManager.Redraw(levelData, mainView);
            OnSelectedLayerChange(_selectedLayer);
        };

        extraPanel.Add(aiPanel);
        aiPanel.style.display = DisplayStyle.None;

        // Gen3DPanel
        gen3DPanel = new Generator3DPanel();
        extraPanel.Add(gen3DPanel);
        gen3DPanel.style.display = DisplayStyle.None;
        gen3DPanel.OnExecute = () =>
        {
            gen3DPanel.Init(_selectedLayer);
        };

        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            var value = (layerPanel.style.display == DisplayStyle.None);
            layerPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            TryCollapseMenuPanels();
        };

        // IAButton
        var IABtn = rootVisualElement.Q<Button>("AIButton");
        IABtn.clicked += () =>
        {
            aiPanel.Init(_selectedLayer);
            var value = (aiPanel.style.display == DisplayStyle.None);
            aiPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            TryCollapseMenuPanels();
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            gen3DPanel.Init(_selectedLayer);
            var value = (gen3DPanel.style.display == DisplayStyle.None);
            gen3DPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            TryCollapseMenuPanels();
        };

        LBSController.OnLoadLevel += (l) => _selectedLayer = null;
        //levelData.OnReload += (l) => _selectedLayer = null;
        //levelData.OnReload += (l) => Debug.Log(_selectedLayer);
    }

    private void ShowinfoLayer(LBSLayer layer)
    {
        if (!layer.Equals(_selectedLayer))
        {
            OnSelectedLayerChange(layer);
            gen3DPanel.Init(layer);
        }

        if (_selectedLayer != null)
        {
            var il = Reflection.MakeGenericScriptable(_selectedLayer);
            Selection.SetActiveObjectWithContext(il, il);
        }
    }

    private void TryCollapseMenuPanels()
    {
        if (layerPanel?.style.display == DisplayStyle.None &&
            aiPanel?.style.display == DisplayStyle.None &&
            gen3DPanel?.style.display == DisplayStyle.None)
        {
            floatingPanelContent.style.display = DisplayStyle.None;
        }
        else
        {
            floatingPanelContent.style.display = DisplayStyle.Flex;
        }
    }

    private new void Repaint()
    {
        base.Repaint();
        drawManager.Redraw(levelData, mainView);
    }

    private void RefreshWindow()
    {
        mainView.Clear();
        this.rootVisualElement.Clear();
        Init();
        mainView.OnClearSelection?.Invoke();
    }

    private void OnLevelDataChange(LBSLevelData levelData)
    {
        noLayerSign.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.Flex : DisplayStyle.None;
    }

    private void OnApplyTrasformers(string modeFrom, string modeTo)
    {
        /*
        var ModuleFrom = _selectedLayer.GetMode(layerTemplates, modeFrom).module;
        var ModuleTo = _selectedLayer.GetMode(layerTemplates, modeTo).module;

        var transformers = _selectedLayer.GetTrasformers(layerTemplates);
        var trans = transformers.Find(t => t.From.FullName.Equals(ModuleFrom) && t.To.FullName.Equals(ModuleTo)); // (!) lo de los fullname es parche ya que ".ModuleType no funciona"
        
        if(trans != null)
        {
            trans.Switch(ref _selectedLayer);
        }*/
    }
    /*
    public void OnModeUpdate(LBSLayer layer)
    {
        var transformers = layer.GetTrasformers(layerTemplates);
        if (transformers.Count <= 0)
            return;
        var t = transformers.First();
        t.ReCalculate(ref layer);

        var templates = layerTemplates.Where(l => l.layer.ID == _selectedLayer.ID).ToList();
        var allModes = templates.SelectMany(l => l.modes).ToList();

        /*var modes = new List<LBSMode>();

        foreach(var mod in allModes)
        {
            if(mod.module == t.To.FullName)
            {
                modes.Add(mod);
            }
        }

        var modes = allModes.Where(m => m.module == t.To.FullName).ToList();
        var modesID = modes.Where(m => m.name != _selectedMode).Select(m => m.name).ToList();

        if (modesID.Count() <= 0)
        {
            drawManager.RefreshView(ref _selectedLayer, levelData.Layers, _selectedMode);
            return;
        }

        var m = modesID.First();
        

        modeSelector.Index = modeSelector.GetChoiceIndex(m);
    }*/
    /*
    public void OnSelectedModeChange(string mode, LBSLayer layer)
    {
        _selectedLayer = layer;

        var oldMode = _selectedMode;
        _selectedMode = mode;
        var modes = _selectedLayer.GetToolkit(layerTemplates);

        // Init tools
        object tools = null;
        modes.TryGetValue(mode,out tools);
        var module = layer.GetModule(0); // (!!) implementar cuando se pueda seleccionar un modulo
        toolkitManager.SetTools(tools, ref levelData, ref layer, ref module);
        modeSelector.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.None : DisplayStyle.Flex;

        drawManager.RefreshView(ref _selectedLayer,levelData.Layers, _selectedMode);
    }*/

    private void OnSelectedLayerChange(LBSLayer layer)
    {
        _selectedLayer = layer;
        inspectorManager.OnSelectedLayerChange(layer);
        //toolkitManager.OnSelectedLayerChange(layer);
        //OnSelectedModeChange(modes.Keys.First(), _selectedLayer);

        //Actualiza AI Panel
        aiPanel.Clear();
        aiPanel.Init(layer);

        selectedLabel.text = "selected: " + layer.Name;
    }
}
