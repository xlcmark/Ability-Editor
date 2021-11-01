
using System;
using System.Reflection;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(EnumFlagsAttribute))]
public class EnumFlagsAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Enum targetEnum = GetBaseProperty<Enum>(property);


        Enum enumNew = EditorGUI.EnumPopup(position,label, targetEnum);
        property.intValue = (int)Convert.ChangeType(enumNew, targetEnum.GetType());
       


    }
    T GetBaseProperty<T>(SerializedProperty prop)
    {
        // Separate the steps it takes to get to this property
        string[] separatedPaths = prop.propertyPath.Split('.');

        // Go down to the root of this serialized property
        System.Object reflectionTarget = prop.serializedObject.targetObject as object;
        // Walk down the path to get the target object
        foreach (var path in separatedPaths)
        {
            FieldInfo fi = reflectionTarget.GetType().GetField(path);
            reflectionTarget = fi.GetValue(reflectionTarget);
        }
        return (T)reflectionTarget;
    }



}
