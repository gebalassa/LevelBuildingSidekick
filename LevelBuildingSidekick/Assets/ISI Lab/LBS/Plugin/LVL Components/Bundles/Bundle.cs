using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public abstract class Bundle : ScriptableObject
{

    [SerializeField]
    [ScriptableToString(typeof(LBSIdentifier))]
    protected string id = "";

    public LBSIdentifier ID
    {
        get => Utility.DirectoryTools.GetScriptable<LBSIdentifier>(id);
        set => id = value.name;
    }

    [SerializeField, SerializeReference]
    protected List<LBSCharacteristic> characteristics;

    public abstract void Add(List<Bundle> data);
    public abstract LBSCharacteristic GetTag(int index);
    public abstract List<LBSCharacteristic> GetCharacteristics();
    public abstract GameObject GetObject(int index);
    public abstract List<GameObject> GetObjects(List<string> tags = null);
    public abstract void Remove(List<Bundle> data);
}
