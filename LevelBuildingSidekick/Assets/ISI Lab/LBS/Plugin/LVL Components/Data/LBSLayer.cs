using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LBS.Tools.Transformer;
using System;
using System.Linq;
using LBS.AI;
using LBS.Settings;
using LBS.Generator;
using LBS.Behaviours;
using LBS.Assisstants;
using UnityEngine.EventSystems;
using static UnityEditor.Experimental.GraphView.GraphView;

namespace LBS.Components
{
    [System.Serializable]
    public class LBSLayer : ICloneable
    {
        #region META-FIELDS
        [SerializeField, JsonRequired]
        private bool visible = true;

        [SerializeField, JsonRequired]
        private bool blocked = false;

        [PathTexture]
        [SerializeField, JsonRequired]
        public string iconPath = "Icon/Default";
        #endregion

        #region FIELDS
        [SerializeField, JsonRequired]
        private string id = "Default ID"; // asegurarse que se usa (!)

        [SerializeField, JsonRequired]
        private string name = "Layer name";

        [JsonRequired]
        private LBSLevelData parent; // esto es ignorable??

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSModule> modules = new List<LBSModule>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSBehaviour> behaviours = new List<LBSBehaviour>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSAssistant> assitants = new List<LBSAssistant>();

        [SerializeField, JsonRequired, SerializeReference]
        private List<LBSGeneratorRule> generatorRules = new List<LBSGeneratorRule>();

        [SerializeField, JsonRequired]
        private Generator3D.Settings settings;
        #endregion

        #region META-PROPERTIES
        [JsonIgnore]
        public bool IsVisible
        {
            get => visible;
            set => visible = value;
        }

        [JsonIgnore]
        public bool IsBlocked
        {
            get => blocked;
            set => blocked = value;
        }
        #endregion

        #region PROPERTIES
        [JsonIgnore]
        public bool IsLocked
        {
            get => blocked;
            set => blocked = value;
        }

        [JsonIgnore]
        public LBSLevelData Parent
        {
            get => parent;
            set => parent = value;
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
        public List<LBSModule> Modules
        {
            get => new List<LBSModule>(modules);
        }

        [JsonIgnore]
        public List<LBSBehaviour> Behaviours
        {
            get => new List<LBSBehaviour>(behaviours);
        }

        [JsonIgnore]
        public List<LBSAssistant> Assitants
        {
            get => new List<LBSAssistant>(assitants);
        }

        [JsonIgnore]
        public List<LBSGeneratorRule> GeneratorRules
        {
            get => new List<LBSGeneratorRule>(generatorRules);
        }

        [JsonIgnore]
        public Generator3D.Settings Settings
        {
            get => settings;
            set => settings = value;
        }

        [JsonIgnore]
        public Vector2Int TileSize
        {
            get
            {
                return new Vector2Int(
                    (int)settings.scale.x,
                    (int)settings.scale.y);
            }
            set
            {
                settings.scale.x = value.x;
                settings.scale.y = value.y;
                OnTileSizeChange?.Invoke(value);
            }
        }
        #endregion

        #region EVENTS
        public event Action<Vector2Int> OnTileSizeChange;
        public event Action<LBSLayer, LBSModule> OnAddModule;
        public event Action<LBSLayer, LBSModule> OnRemoveModule;
        #endregion

        #region  CONSTRUCTORS
        public LBSLayer()
        {
            modules = new List<LBSModule>();
            
            IsVisible = true;
            ID = GetType().Name;
        }

        public LBSLayer(
            IEnumerable<LBSModule> modules,
            IEnumerable<LBSAssistant> assistant,
            IEnumerable<LBSGeneratorRule> rules,
            IEnumerable<LBSBehaviour> behaviours,
            string ID, bool visible, string name, string iconPath, Vector2Int tileSize)
        {
            foreach (var m in modules)
            {
                AddModule(m);
            }

            foreach(var a in assistant)
            {
                AddAssistant(a);
            }

            foreach(var r in rules)
            {
                AddGeneratorRule(r);
            }

            foreach(var b in behaviours)
            {
                AddBehaviour(b);
            }

            this.ID = ID;
            IsVisible = visible;
            this.name = name;
            this.iconPath = iconPath;
            this.TileSize = tileSize;
        }
        #endregion

        #region  METHODS
        public void ReplaceModule(LBSModule oldModule, LBSModule newModule)
        {
            var index = modules.IndexOf(oldModule);
            RemoveModule(oldModule);
            modules.Insert(index, newModule);
            OnAddModule?.Invoke(this, newModule);
        }

        public void Reload()
        {
            foreach (var module in modules)
            {
                module.OnAttach(this);
            }

            foreach(var assistant in assitants)
            {
                assistant.OnAttachLayer(this);
            }

            foreach (var rule in generatorRules)
            {
                // rule.OnAttachLayer(this);
            }

            foreach (var behaviour in behaviours)
            {
                behaviour.OnAttachLayer(this);
            }
        }

        public void AddBehaviour(LBSBehaviour behaviour)
        {
            if (this.behaviours.Contains(behaviour))
            {
                Debug.Log("[ISI Lab]: This layer already contains the behavior " + behaviour.GetType().Name + ".");
                return;
            }

            this.behaviours.Add(behaviour);
            behaviour.OnAttachLayer(this);

            // check if the layer have necesarie 'Modules'
            var reqModules = behaviour.GetRequieredModules();
            foreach (var type in reqModules)
            {
                // aqui podria ser importante preguntar por una key en particular por si
                // existen dos modulos del mismo tipo pero para cosas diferetnes (!!)
                if (!modules.Any(e => e.GetType() == type))         
                {
                    this.AddModule(Activator.CreateInstance(type) as LBSModule);
                }
            }

        }

        public void RemoveBehaviour(LBSBehaviour behaviour)
        {
            this.behaviours.Remove(behaviour);
            behaviour.OnDetachLayer(this);
        }

        public void AddGeneratorRule(LBSGeneratorRule rule)
        {
            this.generatorRules.Add(rule);
            //generatorRules.onAttachLayer(this);
        }

        public bool RemoveGeneratorRule(LBSGeneratorRule rule)
        {
            return this.generatorRules.Remove(rule);
        }

        public void AddAssistant(LBSAssistant assistant)
        {
            if (this.assitants.Find( a => assistant.GetType().Equals(a.GetType())) != null)
            {
                Debug.Log("[ISI Lab]: This layer already contains the assistant " + assistant.GetType().Name + ".");
                return;
            }

            this.assitants.Add(assistant);
            assistant.OnAttachLayer(this);

            var reqModules = assistant.GetRequieredModules();
            foreach (var type in reqModules)
            {
                // aqui podria ser importante preguntar por una key en particular por si
                // existen dos modulos del mismo tipo pero para cosas diferetnes (!!)
                if (!modules.Any(e => e.GetType() == type))
                {
                    this.AddModule(Activator.CreateInstance(type) as LBSModule);
                }
            }

        }

        public void RemoveAssitant(LBSAssistant assistant)
        {
            this.assitants.Remove(assistant);
            assistant.OnDetachLayer(this);
        }

        public LBSAssistant GetAssistant(int index)
        {
            return assitants[index];
        }

        public bool AddModule(LBSModule module)
        {
            if(modules.Contains(module))
            {
                return false;
            }
            modules.Add(module);
            module.OnAttach(this);
            OnAddModule?.Invoke(this, module);
            return true;
        }

        public bool RemoveModule(LBSModule module)
        {
            var removed = modules.Remove(module);
            module.OnDetach(this);
            OnRemoveModule?.Invoke(this,module);
            return removed;
        }

        public LBSModule GetModule(int index)
        {
            return modules[index];
        }

        public LBSModule GetModule(string ID)
        {
            foreach (var module in modules)
            {
                if(module.ID == ID)
                    return module;
            }
            return null;
        }

        public T GetModule<T>(string ID = "") where T : LBSModule
        {
            var t = typeof(T);
            foreach (var module in modules)
            {
                if (module is T || Utility.Reflection.IsSubclassOfRawGeneric(t,module.GetType()))
                {
                    if(ID.Equals("") || module.ID.Equals(ID))
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
                    if (ID.Equals("") || module.ID.Equals(ID))
                    {
                        return module;
                    }
                }
            }
            return null;

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

            index = modules.FindIndex(m => m is T && m.ID.Equals(key));

            modules[index].OnDetach(this);
            modules[index] = module;
            modules[index].OnAttach(this);
            modules[index].Owner = this;
        }

        // esto tiene que ir en una extension (?)
        public Vector2Int ToFixedPosition(Vector2 position) 
        {
            Vector2 pos = position / (TileSize * LBSSettings.Instance.general.TileSize);

            if (pos.x < 0)
                pos.x -= 1;

            if (pos.y < 0)
                pos.y -= 1;

            return pos.ToInt();
        }

        public object Clone()
        {
            var modules = this.modules.Clone(); // CloneRef
            var assistants = this.assitants.Select(a => a.Clone() as LBSAssistant);
            var rules = this.generatorRules.Select(r => r.Clone() as LBSGeneratorRule);
            var behaviours = this.behaviours.Select(b => b.Clone() as LBSBehaviour);

            var layer = new LBSLayer(modules, assistants, rules, behaviours, this.id, this.visible, this.name, this.iconPath, this.TileSize);
            return layer;
        }

        /*
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
            module.OnChanged += (mo) => { this.onModuleChange(this); };
            return true;
        }
        */

        /*
        public LBSModule RemoveModuleAt(int index)
        {
            var module = modules[index];
            RemoveModule(module);
            return module;
        }
        */
        #endregion
    }
}

