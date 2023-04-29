using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[CreateAssetMenu(fileName = "New Composite bundle", menuName = "ISILab/Composite bundle")]
[System.Serializable]
public class CompositeBundle : Bundle_Old
{
    [SerializeReference]
    private List<Bundle_Old> bundles = new List<Bundle_Old>();

    public List<Bundle_Old> Bundles => new List<Bundle_Old>(bundles);

    public override void Add(List<Bundle_Old> data)
    {
        bundles.AddRange(data);
    }

    public void Add(Bundle_Old data)
    {
        bundles.Add(data);
    }

    public override GameObject GetObject(int index)
    {
        int currentIndex = 0;
        foreach (var bundle in bundles)
        {
            List<GameObject> prefabs = bundle.GetObjects();
            if (currentIndex + prefabs.Count > index)
            {
                return prefabs[index - currentIndex];
            }
            currentIndex += prefabs.Count;
        }
        return null;
    }

    public override List<GameObject> GetObjects(List<string> tags = null)
    {
        var ts = TrimTags(tags);

        List<GameObject> objects = new List<GameObject>();
        foreach (Bundle_Old bundle in bundles)
        {
            objects.AddRange(bundle.GetObjects(ts));
        }
        return objects;
    }

    public override LBSCharacteristic GetTag(int index)
    {
        int currentIndex = 0;
        foreach (Bundle_Old bundle in bundles)
        {
            List<LBSCharacteristic> currentTags = bundle.GetCharacteristics();
            if (currentIndex + currentTags.Count > index)
            {
                return currentTags[index - currentIndex];
            }
            currentIndex += currentTags.Count;
        }
        return null;
    }

    public override List<LBSCharacteristic> GetCharacteristics()
    {
        List<LBSCharacteristic> allTags = new List<LBSCharacteristic>();
        foreach (Bundle_Old bundle in bundles)
        {
            allTags.AddRange(bundle.GetCharacteristics());
        }
        return allTags;
    }

    public override void Remove(List<Bundle_Old> data)
    {
        throw new System.NotImplementedException();
    }

    public List<Bundle_Old> GetBundles(string id)
    {
        return bundles.Where(b => b.ID.Label == id).ToList();
    }

    public List<GameObject> GetObjects(string id , List<string> tags = null)
    {
        var ts = TrimTags(tags);

        List<GameObject> objects = new List<GameObject>();
        foreach (var bundle in bundles)
        {
            if (bundle == null)
                continue;

            if (bundle is CompositeBundle)
            {
                objects.AddRange((bundle as CompositeBundle).GetObjects(id, ts));
            }
            else if (bundle.ID?.Label == id)
            {
                objects.AddRange(bundle.GetObjects(ts));
            }
        }
        return objects;
    }

    private List<string> TrimTags(List<string> tags = null)
    {
        var ts = new List<string>();

        if (tags != null)
        {
            ts = tags.Select(s => s.Clone() as string).ToList();
            if(ts.Contains(id?.Label))
            {
                ts.Remove(id.Label);
            }
        }
        return ts;
    }
}
