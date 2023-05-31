using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[LBSCustomEditor(typeof(LBSTagsCharacteristic))]
public class TagsCharEditor : LBSCustomEditor
{
    public TextField labelField;
    public DropdownField dropdownField;

    public LBSTagsCharacteristic target;

    public TagsCharEditor()
    {
        var storage = LBSAssetsStorage.Instance;

        labelField = new TextField();
        this.Add(labelField);
        labelField.RegisterCallback<BlurEvent>(e => {
            target.Label = labelField.value;
        });

        dropdownField = new DropdownField();
        this.Add(dropdownField);
        var tags = storage.Get<LBSIdentifier>();
        dropdownField.choices = tags.Select(t => t.Label).ToList();
        dropdownField.RegisterCallback<ChangeEvent<string>>(e => {
            target.Value = tags.Find(t => t.Label == e.newValue);
        });

    }

    public override void SetInfo(object target)
    {
        this.target = target as LBSTagsCharacteristic;

        labelField.value = this.target?.Label;
        dropdownField.value = this.target?.Value?.Label;
    }
}
