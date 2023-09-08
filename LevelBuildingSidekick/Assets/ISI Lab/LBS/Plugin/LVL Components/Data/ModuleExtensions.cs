using LBS.Components;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public static class ModuleExtensions
{
    public static T GetModule<T>(this List<LBSModule> modules , string ID = "") where T : LBSModule
    {
        var t = typeof(T);
        foreach (var module in modules)
        {
            if (module is T || Utility.Reflection.IsSubclassOfRawGeneric(t, module.GetType()))
            {
                if (ID.Equals("") || module.ID.Equals(ID))
                {
                    return module as T;
                }
            }
        }
        return null;
    }

    public static List<LBSModule> Clone(this List<LBSModule> modules)
    {
        CloneRefs.Start();
        var clone = modules.Select(m => m.Clone() as LBSModule).ToList();
        CloneRefs.End();

        return clone;
    }
}

public static class CloneRefs
{

    private static bool cicloDeClonado = false;
    private static Dictionary<object, object> dictionary = new(); 

    public static void Start() // esto podria recivir una ID para abri diferentes ciclos
    {
        if(cicloDeClonado)
        {
            Debug.LogWarning("No puedes iniciar un ciclo de clona teneindo uno previamente iniciado");
        }

        cicloDeClonado = true;
    }

    public static void End()
    {
        cicloDeClonado = false;
        dictionary.Clear();
    }

    public static void Add(object original, object clone)
    {
        if (!cicloDeClonado)
        {
            Debug.LogWarning("No puedes a�adir si no has iniciado un ciclo de clonado.");
            return;
        }

        dictionary[original] = clone;
    }

    public static object tryGet(object original)
    {
        if (!cicloDeClonado)
        {
            Debug.LogWarning("No puedes obtener si no has iniciado un ciclo de clonado.");
            return null;
        }

        return dictionary[original];
    }

}