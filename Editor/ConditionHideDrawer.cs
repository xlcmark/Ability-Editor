using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(ConditionHideAttribute),true)]
public class ConditionHideDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {

        ConditionHideAttribute ch = attribute as ConditionHideAttribute;

        if (property.serializedObject.FindProperty(ch.ConditionValueName).propertyType==SerializedPropertyType.Enum)
        {
            int num = property.serializedObject.FindProperty(ch.ConditionValueName).intValue;
            if ((num & ch.BelongNumber)!=0)
            {
                return base.GetPropertyHeight(property, label);
            }
        }
        if (property.serializedObject.FindProperty(ch.ConditionValueName).propertyType == SerializedPropertyType.Boolean)
        {
            bool enable =property.serializedObject.FindProperty(ch.ConditionValueName).boolValue;
            if(enable== ch.BelongBool)
            {
                return base.GetPropertyHeight(property, label);
            }
        }

        return -EditorGUIUtility.standardVerticalSpacing;//把間隔刪掉
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {


        ConditionHideAttribute ch = attribute as ConditionHideAttribute;
        //類別為enum
        if (property.serializedObject.FindProperty(ch.ConditionValueName).propertyType == SerializedPropertyType.Enum)
        {
            int num = property.serializedObject.FindProperty(ch.ConditionValueName).intValue;
            if ((num & ch.BelongNumber) != 0)
            {
                position.xMin += 12;
                EditorGUI.PropertyField(position, property, label); 
            }
        }
        //類別為bool
        if (property.serializedObject.FindProperty(ch.ConditionValueName).propertyType == SerializedPropertyType.Boolean)
        {
            bool enable = property.serializedObject.FindProperty(ch.ConditionValueName).boolValue;
            if (enable == ch.BelongBool)
            {
                position.xMin += 12;
                EditorGUI.PropertyField(position, property, label);
            }
        }
    }
}
