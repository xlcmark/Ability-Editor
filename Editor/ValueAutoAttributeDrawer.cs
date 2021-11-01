using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
[CustomPropertyDrawer(typeof(ValueAutoAttribute))]
public class ValueAutoAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        SerializedObject target=null;
        if (property.objectReferenceValue!=null)
            target = new SerializedObject(property.objectReferenceValue);
        string label2 = (target != null) ? target.FindProperty("valueName").stringValue : "Null";
        Rect rect = position;
        rect.xMax = position.xMax - 110;
        EditorGUI.LabelField(rect, label.text,label2,GUI.skin.textField);
        Rect rect1 = position;
        rect1.xMin = position.xMax-100;
        rect1.xMax = position.xMax-5;
        if(GUI.Button(rect1, "Select Value"))
        {
            SelectValue(property);
        }
        Rect rect2 = position;
        rect2.xMax = position.xMax - 110;
        rect2.xMin = rect2.xMax - 20;
        if (GUI.Button(rect2, "x"))
        {
            property.objectReferenceValue = null;
        }
        property.serializedObject.ApplyModifiedProperties();
    }
    private void SelectValue(SerializedProperty prop)
    {
        string dataPath = AssetDatabase.GetAssetPath(prop.serializedObject.targetObject);
        Object[] subObjs = AssetDatabase.LoadAllAssetRepresentationsAtPath(dataPath);
        subObjs= subObjs.Where(x => x.GetType()==typeof(value) || x.GetType().IsSubclassOf(typeof(value))).ToArray() ;//linq
        foreach (value sub in subObjs)
        {
            sub.name = "Ability_value("+sub.valueName+")";
        }
        AssetDatabase.ImportAsset(dataPath);//套用回資源
        ObjectSelectorWindow.ShoWWindow(subObjs, prop);
    }
}
