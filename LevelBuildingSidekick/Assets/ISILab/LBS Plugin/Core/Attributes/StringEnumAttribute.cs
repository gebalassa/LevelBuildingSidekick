using LBS;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using Utility;

public class StringEnumAttribute : PropertyAttribute
{
    public LBSTags DB;

    public StringEnumAttribute()
    {
        DB = DirectoryTools.GetScriptable<LBSTags>();
    }
}

[CustomPropertyDrawer(typeof(StringEnumAttribute))]
public class SEDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var att = attribute as StringEnumAttribute;

        var all = new List<string>(att.DB.Basics).Concat(att.DB.Others).ToList();

        var db = all.Append("Add new...");

        var n = all.IndexOf(property.stringValue);
        var t = EditorGUI.Popup(position, n, db.ToArray());

        if (t < all.Count)
        {
            property.stringValue = all.ToList()[t];
        }
        else
        {
            Selection.activeObject = att.DB;
            EditorGUIUtility.PingObject(att.DB);
            att.DB.AddTag("");

        }
    }
}