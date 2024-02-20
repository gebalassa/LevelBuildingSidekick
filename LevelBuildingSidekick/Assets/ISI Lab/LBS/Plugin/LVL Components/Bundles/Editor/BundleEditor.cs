using LBS.Settings;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ISILab.Commons.Utility;
using ISILab.Commons.Utility.Editor;
using ISILab.Extensions;
using ISILab.LBS.Characteristics;
using UnityEngine.UIElements;
using ISILab.LBS.Components;
using UnityEditor.UIElements;
using LBS.Bundles;
using System;
using ISILab.LBS.AI.Categorization;

namespace ISILab.LBS.Bundles.Editor
{
    [CustomEditor(typeof(Bundle))]
    public class BundleEditor : UnityEditor.Editor
    {
        ListView characteristics;

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
        }

        public override VisualElement CreateInspectorGUI()
        {
            var root = new VisualElement();
            InspectorElement.FillDefaultInspector(root, this.serializedObject, this);

            var bundle = target as Bundle;

            //Bundle Characteristics Lists
            characteristics = new ListView();
            characteristics.headerTitle = "Characteristics";
            characteristics.showAddRemoveFooter = true;
            characteristics.showBorder = true;
            characteristics.showFoldoutHeader = true;
            characteristics.virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight;

            characteristics.makeItem = MakeItem;
            characteristics.bindItem = BindItem;

            characteristics.itemsSource = bundle.characteristics;

            root.Insert(root.childCount - 1, characteristics);

            return root;
        }

        VisualElement MakeItem()
        {
            var bundle = target as Bundle;
            var v = new DynamicFoldout(typeof(LBSCharacteristic));
            return v;
        }

        void BindItem(VisualElement ve, int index)
        {
            var bundle = target as Bundle;
            if (index < bundle.characteristics.Count)
            {
                var cf = ve.Q<DynamicFoldout>();
                cf.Label = "Characteristic " + index + ":";
                //Debug.Log("Bind");
                if (bundle.characteristics[index] != null)
                {
                    //Debug.Log(eval.resourceCharactersitic[index]);
                    cf.Data = bundle.characteristics[index];
                }
                cf.OnChoiceSelection = () => { bundle.characteristics[index] = cf.Data as LBSCharacteristic; };
                //cf.OnChoiceSelection += Save;
            }
        }
        public void Save()
        {
            EditorUtility.SetDirty(target);
        }

        private void OnDisable()
        {
            EditorUtility.SetDirty(target);
        }

        private void OnDestroy()
        {
            EditorUtility.SetDirty(target);
        }
    }
}