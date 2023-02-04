using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Tools.Transformer;
using System;
using System.Linq;

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer : ICloneable
    {

        [SerializeField, JsonRequired]
        private string id;

        [SerializeField, JsonRequired]
        private string name;

        [SerializeField, JsonRequired]
        private bool visible;

        [SerializeField, JsonRequired]
        public string iconPath; // (?) esto tiene que estar en la layertemplate

        //[SerializeField, JsonRequired, SerializeReference]
        //private List<string> transformers; 

        //[SerializeField, JsonRequired, SerializeReference]
        //private List<Transformer> transformers;

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules;

        [SerializeField, JsonRequired]
        [ScriptableToString(typeof(CompositeBundle))]
        private string bundle = "";

        [SerializeField, JsonRequired]
        [ScriptableToString(typeof(LBSLayerAssistant))]
        private string assitant = "";

        [JsonIgnore]
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }

        [JsonIgnore]
        public string ID
        {
            get => id;
            set => id = value;
        }

        [JsonIgnore]
        public string Name
        {
            get => name;
            set => name = value;
        }


        [JsonIgnore]
        public LBSLayerAssistant Assitant
        {
            get 
            {
                /*
                var obj = Resources.FindObjectsOfTypeAll<LBSLayerAssistant>().ToList();
                if(string.IsNullOrEmpty(assitant)  && obj.Count > 0)
                    assitant = obj[0].name;
                var t = obj.Find(a => a.name.Contains(assitant)); // Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>(assitant);
                return t;*/

                if (string.IsNullOrEmpty(assitant))
                    bundle = Utility.DirectoryTools.GetScriptables<LBSLayerAssistant>()[0].name;
                return Utility.DirectoryTools.GetScriptable<LBSLayerAssistant>(assitant);
            }
        }

        [JsonIgnore]
        public CompositeBundle Bundle
        {
            get
            {
                /*var obj = Resources.LoadAll<CompositeBundle>("Bundles").ToList();
                if (string.IsNullOrEmpty(bundle))
                    bundle = obj[0].name;
                var t = obj.Find(a => a.name.Contains(bundle)); //Utility.DirectoryTools.GetScriptable<CompositeBundle>(bundle);
                return t;*/

                if (string.IsNullOrEmpty(bundle))
                    bundle = Utility.DirectoryTools.GetScriptables<CompositeBundle>()[0].name;
                return Utility.DirectoryTools.GetScriptable<CompositeBundle>(bundle);
            }
        }

        //EVENTS
        [JsonIgnore]
        public Action<LBSLayer> OnChanged;

        //CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            
            //transformers = new List<string>();
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(List<LBSModule> modules,/* List<Type> transformers,*/ string ID, bool visible, string name, string iconPath)
        {
            this.modules = modules;
            modules.ForEach(m => {
                AddModule(m);
            });
            //this.transformers = new List<string>();
            //AddTrasformers(transformers);
            this.ID = ID;
            IsVisible = visible;
            this.name = name;
            this.iconPath = iconPath;
        }

        //METHODS
        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
            module.Owner = this;
            module.OnChanged += (mo) => 
            { 
                this.OnChanged?.Invoke(this); 
            };
            return true;
        }

        public bool InsertModule(int index, LBSModule module)
        {
            if (modules.Contains(module))
            {
                return false;
            }
            if (!(modules.ContainsIndex(index) || index == modules.Count))
            {
                return false;
            }
            modules.Insert(index, module);
            module.Owner = this;
            module.OnChanged += (mo) => { this.OnChanged(this); };
            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            var removed = modules.Remove(module);
            if(removed)
            {
                module.Owner = null;
                module.OnChanged -= (mo) => { this.OnChanged(this); };
            }
            return removed;
        }

        public LBSModule RemoveModuleAt(int index)
        {
            var module = modules[index];
            RemoveModule(module);
            return module;
        }

        public LBSModule GetModule(int index)
        {
            return modules[index];
        }

        public T GetModule<T>(string ID = "") where T : LBSModule
        {
            var t = typeof(T);
            foreach (var module in modules)
            {
                if (module is T || Utility.Reflection.IsSubclassOfRawGeneric(t,module.GetType()))
                {
                    if(ID.Equals("") || module.Key.Equals(ID))
                    {
                        return module as T;
                    }
                }
            }
            return null;
        }

        public object GetModule(Type type ,string ID = "")
        {
            foreach (var module in modules)
            {
                if (module.GetType().Equals(type) || Utility.Reflection.IsSubclassOfRawGeneric(type, module.GetType()))
                {
                    if (ID.Equals("") || module.Key.Equals(ID))
                    {
                        return module;
                    }
                }
            }
            return null;

        }

        public List<T> GetModules<T>(string ID = "") where T : LBSModule  // (?) sobra?
        {
            List<T> mods = new List<T>();
            foreach (var mod in modules)
            {
                if (ID.Equals("") || mod.Key.Equals(ID))
                    mods.Add(mod as T);
            }
            return mods;
        }

        public void HideModule(int index)
        {
            modules[index].IsVisible = false;
        }

        public void ShowModule(int index)
        {
            modules[index].IsVisible = true;
        }

        internal void SetModule<T>(T module, string key = "") where T : LBSModule
        {
            var index = -1;
            if (key.Equals(""))
            {
                index = modules.FindIndex(m => m is T);
                modules[index] = module;
                return;
            }

            index = modules.FindIndex(m => m is T && m.Key.Equals(key));
            modules[index] = module;



        }

        /*
        public List<Transformer> GetTransformers()
        {
            var toR = new List<Transformer>();
            for (int i = 0; i < transformers.Count; i++)
            {
                var t = GetTransformer(i);
                toR.Add(t);
            }
            return toR;
        }
        */

        /*
        public Transformer GetTransformer(int index)
        {
            var sName = transformers[index];
            var iType = Type.GetType(sName);
            return Activator.CreateInstance(iType) as Transformer;
        }

        public void AddTrasformers(List<Type> trasformers)
        {
            foreach (var trans in trasformers)
            {
                AddTransformer(trans);
            }
        }

        public bool AddTransformer(Type transformer)
        {
            var tName = transformer?.FullName;

            if (transformers.Contains(tName))
            {
                return false;
            }

            transformers.Add(tName);
            return true;
        }

        public bool RemoveTransformer(Type transformer)
        {
            var tName = transformer?.FullName;
            return transformers.Remove(tName);
        }

        public Transformer RemoveTransformerAt(int index)
        {
            var trans = transformers[index];
            transformers.RemoveAt(index);
            var iType = Type.GetType(trans);
            return Activator.CreateInstance(iType) as Transformer;
        }

        */

        public object Clone()
        {
            var modules = this.modules.Select(m => {
                var module = m.Clone() as LBSModule;
                return module;
                }).ToList();
            //var transformers = this.GetTransformers(); // (??) usar clone en vez de pasar la lista?
            var layer = new LBSLayer(modules,/* transformers.Select(t => t.GetType()).ToList(),*/ this.id, this.visible, this.name, this.iconPath);
            return layer;
        }
    }
}

