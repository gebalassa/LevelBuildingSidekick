using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class BundleTagView : VisualElement
{
    public LBSIdentifierBundle tagsBundle;

    // VisualElements
    private TextField bundleNameField;
    private VisualElement content;
    private TextField addField;
    private Button addButton;
    private List<TagView> tagViews;

    public BundleTagView(LBSIdentifierBundle bundle)
    {
        this.tagsBundle = bundle;

        var visualTree = Utility.DirectoryTools.SearchAssetByName<VisualTreeAsset>("BundleTagView"); // Editor
        visualTree.CloneTree(this);

        // BundleNameField
        bundleNameField = this.Q<TextField>("BundleNameField");
        bundleNameField.RegisterCallback<ChangeEvent<string>>(e => OnTextChange(e.newValue));

        // Content
        content = this.Q<VisualElement>("Content");
        var tags = bundle.Tags;
        foreach (var tag in tags)
        {
            var v = new TagView(tag);
            tagViews.Add(v);
            content.Add(v);
        }

        // AddField
        addField = this.Q<TextField>("NewField");

        // AddTagButton
        addButton = this.Q<Button>("AddButton");
        addButton.clicked += () =>
        {
            var v = addField.text;
            if (v == null || v == "")
                return;

            AddTag(v);
        }; 

    }

    private void OnTextChange(string value)
    {
        tagsBundle.name = value;
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
    }

    private void AddTag(string name)
    {
        var so = ScriptableObject.CreateInstance<LBSIdentifier>();
        so.Label = name;
        tagsBundle.AddTag(so);

        string path = "Assets/" + name + "_Tag" + ".asset";
        AssetDatabase.CreateAsset(so, path);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = so;
    }
}
