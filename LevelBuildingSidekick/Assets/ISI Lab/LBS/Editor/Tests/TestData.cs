using LBS.Components;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Playables;
using UnityEngine;
using Utility;
using System.IO;
using LBS.Settings;
using System.Linq;
using LBS.Components.TileMap;
using UnityEngine.Tilemaps;

public class TestData
{
    // Test folder path
    private static string path = LBSSettings.Instance.test.TestFolderPath;

    [Test]
    public void Save_And_Load_Empty_Level_Data()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Empty_Level.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Empty_Level.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Empty_Layer()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        lvl.AddLayer(new LBSLayer());

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Empty_Layer.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Empty_Layer.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_TileMap_Module()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add TileMap
        var tileMap = new TileMapModule();
        layer.AddModule(tileMap);

        // Add some data
        tileMap.AddTile(new LBSTile(new Vector2(0, 0)));

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_TileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_TileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Connected_TileMap()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add TileMap module
        var tileMap = new TileMapModule();
        layer.AddModule(tileMap);

        // Add ConnectedTileMap module
        var connectedTileMap = new ConnectedTileMapModule();
        layer.AddModule(connectedTileMap);

        // Add some data
        var tile = new LBSTile(new Vector2(0, 0));
        tileMap.AddTile(tile);
        connectedTileMap.AddPair(tile, new List<string>() { "Grass", "Path", "Grass", "Path" }, new List<bool>() { true, true, true, true });

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_ConnectedTileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_ConnectedTileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }

    [Test]
    public void Save_And_Load_Sectorized_TileMap()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Add an empty layer to it
        var layer = new LBSLayer();
        lvl.AddLayer(layer);

        // Add TileMap module
        var tileMap = new TileMapModule();
        layer.AddModule(tileMap);

        // Add Sectorized module
        var sectoerized = new SectorizedTileMapModule();
        layer.AddModule(sectoerized);

        // Add some data
        var tile = new LBSTile(new Vector2(0, 0));
        tileMap.AddTile(tile);
        var zone = new Zone("Zone-1", Color.red);
        sectoerized.AddZone(zone);
        sectoerized.AddTile(tile, zone); 
        
        // Save the level as JSON
        JSONDataManager.SaveData(path, "Layer_With_SectorizedTileMap.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Layer_With_SectorizedTileMap.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);
    }


    [Test]
    public void Save_And_Load_Interior_Layer()
    {
        // Create a level
        var lvl = new LBSLevelData();

        // Get interior presset
        var template = LBSAssetsStorage.Instance.Get<LayerTemplate>().First(t => t.name.Contains("Interior"));

        // Clone Interior presset
        var layer = template.layer.Clone() as LBSLayer;
        lvl.AddLayer(layer);

        // Add some data
        var schemaBH = layer.Behaviours[0] as SchemaBehaviour;
        schemaBH.AddZone();
        schemaBH.AddZone();
        schemaBH.AddTile(new Vector2Int(0, 0), schemaBH.Zones[0]);

        // Save the level as JSON
        JSONDataManager.SaveData(path, "Interiror_Layer.tst", lvl);

        // Load the level from JSON
        var loaded = JSONDataManager.LoadData<LBSLevelData>(path, "Interiror_Layer.tst");

        // Check if loaded level is not null
        Assert.IsNotNull(loaded);

        // Cheack if the new level and previously are equals
        Assert.AreEqual(lvl, loaded);

    }

    [Test]
    public void Save_And_Load_Exterior_Layer()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void Save_And_Load_Population_Layer()
    {
        Assert.IsTrue(false);
    }

    [Test]
    public void Save_And_Load_XXX_Layer()
    {
        Assert.IsTrue(false);
    }

    // [TEST] Chequear que las referencias entre modulos sigan funcionando
    // [TEST] Cheaquear que los eventos esten conectados cuando corresponda
    // [TEST] XXX
    // [TEST] XXX
    // [TEST] XXX
    // [TEST] XXX
    // [TEST] XXX

}