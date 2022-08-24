using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New WindowsPreset", menuName ="LBS.../Presets.../WindowsPreset", order =0)]
public class WindowsPreset : ScriptableObject
{
    [SerializeField] 
    private List<string> windows = new List<string>();

    public List<string> Windows => new List<string>(windows); // no se puede pedir la lista entera para editarla solo para leerla

    public void AddWindow(string idWindow)
    {
        windows.Add(idWindow);
    }

    public void RemoveWindow(string idWindow)
    {
        windows.Remove(idWindow);
    }
}
