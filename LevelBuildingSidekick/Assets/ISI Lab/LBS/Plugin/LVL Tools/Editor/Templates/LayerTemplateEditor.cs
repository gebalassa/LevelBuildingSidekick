using LBS.Components;
using LBS.Components.Graph;
using LBS.Components.Specifics;
using LBS.Components.TileMap;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LayerTemplate))]
public class LayerTemplateEditor : Editor
{
    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        var template = (LayerTemplate)target;

        if(GUILayout.Button("Set as interior"))
        {
            InteriorConstruct(template);
        }

        if (GUILayout.Button("Set as exterior"))
        {
            ExteriorConstruct(template);
        }

        if (GUILayout.Button("Set as population"))
        {
            PopulationConstruct(template);
        }
    }

    /// <summary>
    /// 
    /// This function adjust the icons, layout and labels of the system for Contructi�n in interior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void InteriorConstruct(LayerTemplate template)
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSRoomGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Interior";
        layer.Name = "Interior layer";
        layer.iconPath = "Icons/interior-design";

        template.layer = layer;

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Empty), null, true);
        icon = Resources.Load<Texture2D>("Icons/Addnode");
        var tool2 = new LBSTool(icon, "Add node", typeof(CreateNewRoomNode), null, false);
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSTool(icon, "Add conection", typeof(CreateNewConnection<RoomNode>), null, false);
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool4 = new LBSTool(icon, "Remove", typeof(RemoveGraphNode<RoomNode>), null, false);


        var mode1 = new LBSMode("Graph", new DrawSimpleGraph(), new List<LBSTool>() { tool1, tool2, tool3, tool4 });
        template.modes.Add(mode1);

        // Mode 2
        icon = Resources.Load<Texture2D>("Icons/Select");
        var tool5 = new LBSTool(icon, "Select", typeof(Empty), null, true);
       
        var tool6 = new LBSMultiTool(
            icon,
            "Paint tile",
            new List<string>() { "point", "Line", "Grid","Free" },
            new List<System.Type>() { 
                typeof(AddTileToTiledAreaAtPoint<TiledArea<LBSTile>,LBSTile>), // point // (!!) implementar
                typeof(AddTileToTiledAreaAtLine<TiledArea<LBSTile>,LBSTile>), // line // (!!) implementar
                typeof(AddTileToTiledAreaAtGrid<TiledArea<LBSTile>,LBSTile>), // grid // (!!) implementar
                typeof(AddTileToTiledAreaAtFree<TiledArea<LBSTile>,LBSTile>)  // free // (!!) implementar
            },
            typeof(RoomsPalleteInspector<TiledArea<LBSTile>,LBSTile>)
        );
        icon = Resources.Load<Texture2D>("Icons/erase");
        var tool7 = new LBSMultiTool(
            icon,
            "Erase",
            new List<string>() { "point", "Line", "Grid", "Free" },
            new List<System.Type>() {
                typeof(Empty), //typeof(AddTileToTiledAreaAtPoint<TiledArea<LBSTile>,LBSTile>), // point // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtLine<TiledArea<LBSTile>,LBSTile>), // line // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtGrid<TiledArea<LBSTile>,LBSTile>), // grid // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtFree<TiledArea<LBSTile>,LBSTile>)  // free // (!!) implementar
            },
            null
        );

        var mode2 = new LBSMode("Schema", new DrawConnectedTilemap(),new List<LBSTool>() { tool5, tool6 });
        template.modes.Add(mode2);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// This function adjust the icons, layout and labels of the system for Contructi�n in exterior
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void ExteriorConstruct(LayerTemplate template)
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Exterior";
        layer.Name = "Exterior layer";
        layer.iconPath = "Icons/pine-tree";

        template.layer = layer;

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Empty), null, true);
        icon = Resources.Load<Texture2D>("Icons/RoomSelection");
        var tool2 = new LBSTool(icon, "Paint by tile", typeof(Empty), null, false);
        icon = Resources.Load<Texture2D>("Icons/AddConnection");
        var tool3 = new LBSMultiTool(
            icon,
            "Paint by connection",
            new List<string>() { "Point", "Line", "Square", "Free" },
            new List<System.Type>() {
                typeof(Empty), //typeof(AddTileToTiledAreaAtPoint<TiledArea<LBSTile>,LBSTile>), // point // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtLine<TiledArea<LBSTile>,LBSTile>), // line // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtGrid<TiledArea<LBSTile>,LBSTile>), // grid // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtFree<TiledArea<LBSTile>,LBSTile>)  // free // (!!) implementar
            },
            null
        );
        icon = Resources.Load<Texture2D>("Icons/erased");
        var tool4 = new LBSMultiTool(
            icon,
            "Erase",
            new List<string>() { "Pair", "Line", "Square", "Free" },
            new List<System.Type>() {
                typeof(Empty), //typeof(AddTileToTiledAreaAtPoint<TiledArea<LBSTile>,LBSTile>), // point // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtLine<TiledArea<LBSTile>,LBSTile>), // line // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtGrid<TiledArea<LBSTile>,LBSTile>), // grid // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtFree<TiledArea<LBSTile>,LBSTile>)  // free // (!!) implementar
            },
            null
        );


        var mode1 = new LBSMode("Exterior", new DrawSimpleGraph(), new List<LBSTool>() { tool1, tool2, tool3, tool4 });
        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// 
    /// This function adjust the icons, layout and labels of the Population system
    /// also call the manipulators to make functional buttons in the layout
    /// </summary>
    /// <param name="template"></param>
    private void PopulationConstruct(LayerTemplate template)
    {
        var layer = new LBSLayer();

        // Modules
        layer.AddModule(new LBSBaseGraph());

        // Transformers
        //layer.AddTransformer();

        layer.ID = "Population";
        layer.Name = "Population layer";
        layer.iconPath = "Icons/ghost";

        template.layer = layer;

        // Mode 1
        Texture2D icon = Resources.Load<Texture2D>("Icons/Select");
        var tool1 = new LBSTool(icon, "Select", typeof(Empty), null, true);
        icon = Resources.Load<Texture2D>("Icons/paintbrush");
        var tool2 = new LBSMultiTool(
            icon,
            "Add population",
            new List<string>() { "Pair", "Line", "Square", "Free" },
            new List<System.Type>() {
                typeof(Empty), //typeof(AddTileToTiledAreaAtPoint<TiledArea<LBSTile>,LBSTile>), // point // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtLine<TiledArea<LBSTile>,LBSTile>), // line // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtGrid<TiledArea<LBSTile>,LBSTile>), // grid // (!!) implementar
                typeof(Empty), //typeof(AddTileToTiledAreaAtFree<TiledArea<LBSTile>,LBSTile>)  // free // (!!) implementar
            },
            null
        );
        icon = Resources.Load<Texture2D>("Icons/Trash");
        var tool3 = new LBSTool(icon, "Remove", typeof(Empty), null, false);


        var mode1 = new LBSMode("Population", new DrawSimpleGraph(), new List<LBSTool>() { tool1, tool2, tool3});
        template.modes.Add(mode1);

        AssetDatabase.SaveAssets();
    }
}