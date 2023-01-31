using LBS;
using LBS.Components;
using LBS.ElementView;
using LBS.VisualElements;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class LBSMainWindow : EditorWindow
{
    // Data
    public LBSLevelData levelData;

    // Selected
    private LBSLayer _selectedLayer;
    private string _selectedMode; // (??) esto deberia ser mas que un string?

    // Templates
    public List<LayerTemplate> layerTemplates;

    // Visual Elements
    private ButtonGroup toolPanel;
    private VisualElement extraPanel;
    private VisualElement noLayerSign;
    private ModeSelector modeSelector;
    private MainView mainView;
    private Label selectedLabel;

    // Panels
    private LayersPanel layerPanel;
    private AIPanel aiPanel;
    private Generator3DPanel gen3DPanel;

    private AITest AIp;

    // Manager
    private ToolkitManager toolkitManager;
    private DrawManager drawManager;
    private LBSInspectorPanel inspectorManager;


    [MenuItem("ISILab/LBS plugin/Main window", priority = 0)]
    public static void ShowWindow()
    {
        var window = GetWindow<LBSMainWindow>();
        Texture icon = Resources.Load<Texture>("Icons/Logo");
        window.titleContent = new GUIContent("Level builder", icon);
    }

    public virtual void CreateGUI()
    {
        Init();
    }

    public void Init()
    {
        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("LBSMainWindow");
        visualTree.CloneTree(rootVisualElement);

        // LayerTemplate
        layerTemplates = Utility.DirectoryTools.GetScriptablesByType<LayerTemplate>();

        // ToolPanel
        toolPanel = rootVisualElement.Q<ButtonGroup>("ToolsGroup");

        // ModeSelector
        modeSelector = rootVisualElement.Q<ModeSelector>("ModeSelector");
        modeSelector.OnSelectionChange += (mode) =>
        {
            OnApplyTrasformers(_selectedMode, mode);
            OnSelectedModeChange(mode, _selectedLayer);
        };

        // MainView 
        mainView = rootVisualElement.Q<MainView>("MainView");

        // DrawManager
        drawManager = new DrawManager(ref mainView, ref layerTemplates);

        // InspectorContent
        inspectorManager = rootVisualElement.Q<LBSInspectorPanel>("InpectorPanel");

        // ToolKitManager
        toolkitManager = new ToolkitManager(ref toolPanel, ref modeSelector, ref mainView, ref inspectorManager, ref layerTemplates);
        toolkitManager.OnEndSomeAction += () =>
        {
            drawManager.RefreshView(ref _selectedLayer, _selectedMode);
        };

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
            drawManager.RefreshView(ref _selectedLayer, _selectedMode);
        };


        // ExtraPanel
        extraPanel = rootVisualElement.Q<VisualElement>("ExtraPanel");

        // NoLayerSign
        noLayerSign = rootVisualElement.Q<VisualElement>("NoLayerSign");

        // SelectedLabel
        selectedLabel = rootVisualElement.Q<Label>("SelectedLabel");

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
        layerPanel.OnSelectLayer += (layer) =>
        {
            OnSelectedLayerChange(layer);
        };

        /*
        // AIPanel
        aiPanel = new AIPanel(levelData);
        extraPanel.Add(aiPanel);
        aiPanel.style.display = DisplayStyle.None;
        */

        // panel de prueba
        AIp = new AITest(levelData);
        extraPanel.Add(AIp);
        AIp.style.display = DisplayStyle.None;

        // Gen3DPanel
        gen3DPanel = new Generator3DPanel(levelData);
        extraPanel.Add(gen3DPanel);
        gen3DPanel.style.display = DisplayStyle.None;


        // LayerButton
        var layerBtn = rootVisualElement.Q<Button>("LayerButton");
        layerBtn.clicked += () =>
        {
            var value = (layerPanel.style.display == DisplayStyle.None);
            layerPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        // IAButton
        var IABtn = rootVisualElement.Q<Button>("AIButton");
        IABtn.clicked += () =>
        {
            //var value = (aiPanel.style.display == DisplayStyle.None);
            //aiPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;

            var value = (AIp.style.display == DisplayStyle.None);
            AIp.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };

        // 3DButton
        var Gen3DBtn = rootVisualElement.Q<Button>("3DButton");
        Gen3DBtn.clicked += () =>
        {
            var value = (gen3DPanel.style.display == DisplayStyle.None);
            gen3DPanel.style.display = (value) ? DisplayStyle.Flex : DisplayStyle.None;
        };
    }

    private void RefreshWindow()
    {
        mainView.Clear();
        this.rootVisualElement.Clear();
        Init();
    }

    public void OnLevelDataChange(LBSLevelData levelData)
    {
        noLayerSign.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.Flex : DisplayStyle.None;
        modeSelector.style.display = (levelData.Layers.Count <= 0) ? DisplayStyle.None : DisplayStyle.Flex;
    }

    public void OnApplyTrasformers(string modeFrom, string modeTo)
    {
        var ModuleFrom = _selectedLayer.GetMode(layerTemplates, modeFrom).module;
        var ModuleTo = _selectedLayer.GetMode(layerTemplates, modeTo).module;

        var transformers = _selectedLayer.GetTrasformers(layerTemplates);
        var trans = transformers.Find(t => t.From.FullName.Equals(ModuleFrom) && t.To.FullName.Equals(ModuleTo)); // (!) lo de los fullname es parche ya que ".ModuleType no funciona"
        trans?.Switch(ref _selectedLayer);
    }

    public void OnSelectedModeChange(string mode, LBSLayer layer)
    {
        var oldMode = _selectedMode;
        _selectedMode = mode;
        var modes = _selectedLayer.GetToolkit(layerTemplates);

        // Apply trasformer
        // OnApplyTrasformers(oldMode, _selectedMode);

        // Init tools
        object tools;
        modes.TryGetValue(mode,out tools);
        var module = layer.GetModule(0); // (!!) implementar cuando se pueda seleccionar un modulo
        toolkitManager.SetTools(tools, ref levelData, ref layer, ref module);

        drawManager.RefreshView(ref _selectedLayer, _selectedMode);
    }

    public void OnSelectedLayerChange(LBSLayer layer)
    {
        _selectedLayer = layer;
        
        // actualize modes
        var modes = _selectedLayer.GetToolkit(layerTemplates);
        modeSelector.SetChoices(modes);
        modeSelector.Index = 0;
        modeSelector.style.display = DisplayStyle.Flex;
        OnSelectedModeChange(modes.Keys.First(), _selectedLayer);

        selectedLabel.text = "selected: " + layer.Name;

        // (?) Actualize IAs?

        // (?) Actualize gen3D?

    }
}
