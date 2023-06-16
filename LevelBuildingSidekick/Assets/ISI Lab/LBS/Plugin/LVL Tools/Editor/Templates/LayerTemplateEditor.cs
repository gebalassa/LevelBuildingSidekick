using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using LBS.Generator;
using LBS.Tools.Transformer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

[LBSCustomEditor("Layer template",typeof(LayerTemplate))]
[CustomEditor(typeof(LayerTemplate))]
public class LayerTemplateEditor : Editor
{
    private int behaviourIndex = 0;
    private List<Type> behaviourOptions;

    private int assitantIndex = 0;
    private List<Type> assistantOptions;

    private int ruleIndex = 0;
    private List<Type> ruleOptions; 


    void OnEnable()
    {
        behaviourOptions = typeof(LBSBehaviour).GetDerivedTypes().ToList();
        assistantOptions = typeof(LBSAssistantAI).GetDerivedTypes().ToList();
        ruleOptions = typeof(LBSGeneratorRule).GetDerivedTypes().ToList();
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var template = (LayerTemplate)target;

        GUILayout.Space(20);
        GUILayout.BeginHorizontal();
        behaviourIndex = EditorGUILayout.Popup("Type:", behaviourIndex, behaviourOptions.Select(e => e.Name).ToArray());
        var selected = behaviourOptions[behaviourIndex];
        if (GUILayout.Button("Add behaviour"))
        {
            var bh = Activator.CreateInstance(selected);
            template.layer.AddBehaviour(bh as LBSBehaviour);
        }
        GUILayout.EndHorizontal();
        
        GUILayout.BeginHorizontal();
        assitantIndex = EditorGUILayout.Popup("Type:", assitantIndex, assistantOptions.Select(e => e.Name).ToArray());
        var selected2 = assistantOptions[assitantIndex];
        if (GUILayout.Button("Add Assistent"))
        {
            var ass = Activator.CreateInstance(selected2);
            template.layer.AddAssistant(ass as LBSAssistantAI);
        }
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        ruleIndex = EditorGUILayout.Popup("Type:", ruleIndex, ruleOptions.Select(e => e.Name).ToArray());
        var selected3 = ruleOptions[ruleIndex];
        if (GUILayout.Button("Add Assistent"))
        {
            var rule = Activator.CreateInstance(selected3);
            template.layer.AddGeneratorRule(rule as LBSGeneratorRule);
        }
        GUILayout.EndHorizontal();
        GUILayout.Space(20);

        if (GUILayout.Button("Set as Interior")) 
        {
            InteriorConstruct(template);
        }

        if (GUILayout.Button("Set as Exterior"))
        {
            ExteriorConstruct(template);
        }

        if (GUILayout.Button("Set as Population"))
        {
            PopulationConstruct(template);
        }

        if (GUILayout.Button("Set as Quest"))
        {
            Questconstuct(template);
        }
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the system for Contructión in interior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void InteriorConstruct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;
        //layer.TileSize = new Vector2Int(2,2);
        layer.settingsGen3D = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Interior",
        };
        /*
        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("SchemaAssitant");
        if(assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "SchemaAssitant";
            assist.AddAgent(new AssistantHillClimbing(layer, "SchemaHillClimbing"));

            assist.Generator = new Generator3D();
            assist.Generator.AddRule(new SchemaRuleGenerator());

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        */
        layer.AddAssistant(new AssistantHillClimbing());
        layer.AddGeneratorRule(new SchemaRuleGenerator());

        layer.ID = "Interior";
        layer.Name = "Layer Interior";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/interior-design.png";
        template.layer = layer;

        // Modules
        layer.AddModule(new LBSRoomGraph());    // GraphModule<RoomNode>
        layer.AddModule(new LBSSchema());       // AreaTileMap<TiledArea<ConnectedTile>, ConnectedTile>

        // Transformers
        template.transformers.Add(
            new GraphToArea(
                typeof(GraphModule<RoomNode>),
                typeof(AreaTileMap<TiledArea>)
                )
            );

        /*template.transformers.Add(
            new AreaToGraph(
                typeof(AreaTileMap<TiledArea>),
                typeof(GraphModule<RoomNode>)
                )
            );*/

        layer.AddBehaviour(new SchemaBehaviour());

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add Node", typeof(CreateNewRoomNode), null, false);
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewConnection<RoomNode>), null, false);
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool4 = new LBSTool(icon, "Remove Node", typeof(RemoveGraphNode<RoomNode>), null, false);
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool10 = new LBSTool(icon, "Remove Connection", typeof(RemoveGraphConnection), null, false);

        var mode1 = new LBSMode(
            "Graph",
            typeof(GraphModule<RoomNode>)
            , new DrawSimpleGraph(),
            new List<LBSTool>() { tool1, tool2, tool3, tool4, tool10 }
            );
        template.modes.Add(mode1);

        // Mode 2
        icon = Resources.Load<Texture2D>("Icons/Select");
        var tool5 = new LBSTool(icon, "Select", typeof(Select), null, true);

        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        var tool6 = new LBSTool(
            icon,
            "Paint tile",
            typeof(AddTileToTiledArea<TiledArea, ConnectedTile>),
            typeof(RoomsPalleteInspector<TiledArea, ConnectedTile>),
            true);

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool7 = new LBSTool(
            icon,
            "Erase",
            typeof(DeleteTile<TiledArea, ConnectedTile>), // Removed<TiledArea<LBSTile>, LBSTile>,
            null
        );

        icon = Resources.Load<Texture2D>("Icons/open-exit-door");
        var tool8 = new LBSTool(icon, "Add door", typeof(AddDoor<TiledArea,ConnectedTile>), null, true);

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool9 = new LBSTool(
            icon, 
            "Remove door",
            typeof(RemoveDoor<TiledArea,ConnectedTile>), //typeof(RemoveDoor<TiledArea,ConnectedTile>),
            null, 
            true);

        var mode2 = new LBSMode(
            "Schema",
            typeof(AreaTileMap<TiledArea>),
            new DrawConnectedTilemap(),
            new List<LBSTool>() { tool5, tool6, tool7, tool8, tool9 }
            );
        template.modes.Add(mode2);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the system for Contructión in exterior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void ExteriorConstruct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;

        //layer.TileSize = new Vector2Int(10, 10);
        layer.settingsGen3D = new Generator3D.Settings()
        {
            scale = new Vector2Int(10, 10),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Exteriror",
        };
        /*
        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("ExteriorAsstant");
        if (assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "ExteriorAsstant";
            
            assist.Generator = new Generator3D();
            assist.Generator.AddRule(new ExteriorRuleGenerator());

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        */
        layer.AddAssistant(new AssitantWFC());
        layer.AddGeneratorRule(new ExteriorRuleGenerator());

        layer.ID = "Exterior";
        layer.Name = "Layer Exterior";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/pine-tree.png";
        template.layer = layer;

        // Modules
        var x = new Exterior();
        layer.AddModule(x);

        // Transformers
        //
        //

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);


        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool2 = new LBSTool(
            icon,
            "Add empty tile",
            typeof(AddEmptyTile<ConnectedTile>),
            null,
            false);

        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool3 = new LBSTool(
            icon,
            "Remove tile",
            typeof(RemoveTileExterior<ConnectedTile>),
            null,
            false);

        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool4 = new LBSTool(
            icon,
            "Set connection",
            typeof(AddConnection<ConnectedTile>),
            typeof(TagsPalleteInspector), //typeof(RoomsPalleteInspector<TiledArea, ConnectedTile>),
            false);

        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool5 = new LBSTool(
            icon,
            "Remove connection",
            typeof(RemoveConnection<ConnectedTile>), 
            null, 
            false);

        icon = Resources.Load<Texture2D>("Icons/Collapse_Icon");
        var tool6 = new LBSTool(
            icon, 
            "Collapse connection area", 
            typeof(WaveFunctionCollapseManipulator<ConnectedTile>), 
            null, 
            false);

        var mode1 = new LBSMode(
            "Exterior",
            typeof(TiledArea), // (!!!) implentar la correcta
            new DrawExterior(),
            new List<LBSTool>() { tool1, tool2, tool3, tool4, tool5, tool6 }
            );


        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the Population system
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void PopulationConstruct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;

        //layer.TileSize = new Vector2Int(2, 2);
        layer.settingsGen3D = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Population",
        };
        /*
        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("PopulationAssitant");
        if (assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "PopulationAssitant";

            assist.Generator = new Generator3D();
            assist.Generator.AddRule(new PopulationRuleGenerator());

            //(assist.Generator = new PopulationGenerator();
            assist.AddAgent(new AssistantMapElite(layer, "Population Map Elite"));

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        */
        layer.AddAssistant(new AssistantMapElite());
        layer.AddGeneratorRule(new PopulationRuleGenerator());

        layer.ID = "Population";
        layer.Name = "Layer Population";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/ghost.png";
        template.layer = layer;

        // Modules
        layer.AddModule(new LBSTileMap());
        layer.AddModule(new TaggedTileMap());

        // Select
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);

        //Add
        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        var tool2 = new LBSTool(icon, "Add Tile", typeof(AddTaggedTile), typeof(BundlePalleteInspector), false);

        //Remove
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool3 = new LBSTool(icon, "Remove", typeof(RemoveTile), null, false);
        

        var mode1 = new LBSMode(
            "Population",
            //Change to pop
            //Check if 'PopulationTileMap<TiledArea> works
            typeof(TaggedTileMap), 
            new DrawTaggedTileMap(),
            new List<LBSTool>() { tool1, tool2, tool3}
            );
        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="template"></param>
    private void Questconstuct(LayerTemplate template)
    {
        template.Clear();

        // Basic data layer
        var layer = template.layer;

        layer.settingsGen3D = new Generator3D.Settings()
        {
            scale = new Vector2Int(2, 2),
            resize = new Vector2(0, 0),
            position = new Vector3(0, 0, 0),
            name = "Quest",
        };
        /*
        // asistant
        var assist = Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>("QuestAssitant");
        if (assist == null)
        {
            assist = ScriptableObject.CreateInstance<LBSLayerAssistant>();
            assist.name = "QuestAssitant";
            //assist.AddAgent(new QuestAgent(layer, "Quest agent"));

            AssetDatabase.AddObjectToAsset(assist, template);
            AssetDatabase.SaveAssets();
        }

        layer.Assitant = assist;
        */
        layer.AddAssistant(new AssistentGrammar());

        layer.ID = "Quest";
        layer.Name = "Layer Quest";
        layer.iconPath = "Assets/ISI Lab/LBS/Plugin/Assets2D/Resources/Icons/Quest.png";
        template.layer = layer;

        // Modules
        layer.AddModule(new LBSGraph());
        layer.AddModule(new LBSGrammarGraph()); // (!)

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Select), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add node", typeof(CreateNewGrammarNode), typeof(GrammarPallete), false); // (!)
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewConnection<LBSNode>), null, false); // (!)
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool4 = new LBSTool(icon, "Remove", typeof(RemoveGraphNode<LBSNode>), null, false); // (!)

        var mode1 = new LBSMode(
            "Graph",
            typeof(LBSGraph),
            new DrawGrammarGraph(), // (!)
            new List<LBSTool>() { tool1, tool2, tool3, tool4 }
            );
        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }
}
