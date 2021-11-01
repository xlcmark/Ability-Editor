using UnityEngine;

#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEditor;


#endif

#if UNITY_EDITOR
/// <summary>
/// Draws the property field for any field marked with ExpandableAttribute.
/// </summary>
[CustomPropertyDrawer(typeof(ExpandableAttribute), true)]
public class ExpandableAttributeDrawer : PropertyDrawer
{
    // Use the following area to change the style of the expandable ScriptableObject drawers;
    #region Style Setup
    private enum BackgroundStyles
    {
        None,
        HelpBox,
        Darken,
        Lighten
    }

    /// <summary>
    /// Whether the default editor Script field should be shown.
    /// </summary>
    private static bool SHOW_SCRIPT_FIELD = false;

    /// <summary>
    /// The spacing on the inside of the background rect.
    /// </summary>
    private static float INNER_SPACING = 6.0f;

    /// <summary>
    /// The spacing on the outside of the background rect.
    /// </summary>
    private static float OUTER_SPACING = 4.0f;

    /// <summary>
    /// The style the background uses.
    /// </summary>
    private static BackgroundStyles BACKGROUND_STYLE = BackgroundStyles.HelpBox;

    /// <summary>
    /// The colour that is used to darken the background.
    /// </summary>
    private static Color DARKEN_COLOUR = new Color(0.0f, 0.0f, 0.0f, 0.2f);

    /// <summary>
    /// The colour that is used to lighten the background.
    /// </summary>
    private static Color LIGHTEN_COLOUR = new Color(1.0f, 1.0f, 1.0f, 0.2f);
    #endregion

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        float totalHeight = 0.0f;

        totalHeight += EditorGUIUtility.singleLineHeight;

        if (property.objectReferenceValue == null)
            return totalHeight;

        if (!property.isExpanded)
            return totalHeight;

        SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);

        if (targetObject == null)
            return totalHeight;

        SerializedProperty field = targetObject.GetIterator();

        field.NextVisible(true);

        if (SHOW_SCRIPT_FIELD)
        {
            totalHeight += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        while (field.NextVisible(false))
        {
            totalHeight += EditorGUI.GetPropertyHeight(field, true) + EditorGUIUtility.standardVerticalSpacing;
        }

        totalHeight += INNER_SPACING * 2;
        totalHeight += OUTER_SPACING * 2;

       
        return totalHeight;
       
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Rect fieldRect = new Rect(position);
        fieldRect.height = EditorGUIUtility.singleLineHeight;


        #region Mychange
        

         //get type array
        int stringIndex = fieldInfo.FieldType.ToString().IndexOf('[');//欄位類型為generic collection list'[T]
        string typeName = fieldInfo.FieldType.ToString().Substring(stringIndex + 1).TrimEnd(']');
        UnityEngine.Object[]types = Resources.LoadAll("Module", Type.GetType(typeName));
        string[] typeNames = new string[types.Length + 1];
        typeNames[0] = "None";
        for (int i = 0; i < types.Length; i++)
        {
            typeNames[i + 1] = types[i].GetType().ToString();
        }
       
        //繪製按鈕
        ExpandableAttribute ex = attribute as ExpandableAttribute;
        if (ex.IsArray)
        {
            string[] path = property.propertyPath.Split('.');

            string ArrayName = path[0];//尋找陣列路徑(path)
            if (path.Length > 2)
            {
                for (int i = 1; i < path.Length - 2; i++)//陣列為最下層往上兩層
                {
                    ArrayName += "." + path[i];
                }
            }
            SerializedProperty p = property.serializedObject.FindProperty(ArrayName);//得到陣列

            if (p != null && p.isArray)
            {


                if (path[path.Length - 1] == "data[0]")//整個陣列只讀一次
                {
                    //按鈕（＋）
                    Rect PlusButtonPos = fieldRect;
                    PlusButtonPos.y = fieldRect.y - EditorGUIUtility.singleLineHeight- EditorGUIUtility.standardVerticalSpacing;
                    PlusButtonPos.xMax = EditorGUIUtility.labelWidth - 18;
                    PlusButtonPos.xMin = EditorGUIUtility.labelWidth - 48;
                    if (GUI.Button(PlusButtonPos, new GUIContent("+", "ADD")))
                    {
                        p.arraySize++;
                        p.GetArrayElementAtIndex(p.arraySize - 1).objectReferenceValue = null;
                    }

                    //按鈕(-)
                    Rect ReButtonPos = fieldRect;
                    ReButtonPos.y = fieldRect.y - EditorGUIUtility.singleLineHeight- EditorGUIUtility.standardVerticalSpacing;
                    ReButtonPos.xMin = EditorGUIUtility.labelWidth-18;
                    ReButtonPos.xMax = EditorGUIUtility.labelWidth+12;
                    if (GUI.Button(ReButtonPos, new GUIContent("-","Remove")))
                    {
                        //int ArrayIndex = int.Parse(path[path.Length - 1].Substring(5,1));
                        if (p.arraySize - 1 != 0)
                        {
                            //無法刪除最後一個element
                            if(p.GetArrayElementAtIndex(p.arraySize - 1).objectReferenceValue != null)
                            {
                                DestroyModule(p.GetArrayElementAtIndex(p.arraySize - 1).objectReferenceValue, p.GetArrayElementAtIndex(p.arraySize - 1));
                            }
                            p.arraySize--;

                        }
                    }
                }

            }
        }


        EditorGUI.BeginChangeCheck();
           int selected = 0;//沒有來源為0
           if (property.objectReferenceValue != null)
           {
               selected = Array.IndexOf(typeNames, property.objectReferenceValue.GetType().ToString());//獲得來源的index
           }

           selected = EditorGUI.Popup(fieldRect, label.text, selected, typeNames);//顯示popup
        
        if (EditorGUI.EndChangeCheck())
        {
            if (property.objectReferenceValue != null)
                DestroyModule(property.objectReferenceValue, property);
            if (selected != 0)
            {
                property.objectReferenceValue = CreateModule(types[selected - 1], property);

            }

        }
        #endregion


        if (property.objectReferenceValue == null)
            return;

        property.isExpanded = EditorGUI.Foldout(fieldRect, property.isExpanded, GUIContent.none, true);

        if (!property.isExpanded)
            return;

        SerializedObject targetObject = new SerializedObject(property.objectReferenceValue);

        if (targetObject == null)
            return;


        #region Format Field Rects
        List<Rect> propertyRects = new List<Rect>();
        Rect marchingRect = new Rect(fieldRect);

        Rect bodyRect = new Rect(fieldRect);
        bodyRect.xMin += EditorGUI.indentLevel * 14;
        bodyRect.yMin += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing
            + OUTER_SPACING;

        SerializedProperty field = targetObject.GetIterator();
        field.NextVisible(true);

        marchingRect.y += INNER_SPACING + OUTER_SPACING;

        if (SHOW_SCRIPT_FIELD)
        {
            propertyRects.Add(marchingRect);
            marchingRect.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        }

        while (field.NextVisible(false))
        {
            marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
            marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
            propertyRects.Add(marchingRect);
        }

        marchingRect.y += INNER_SPACING;

        bodyRect.yMax = marchingRect.yMax;
        #endregion

        DrawBackground(bodyRect);

        #region Draw Fields
        EditorGUI.indentLevel++;

        int index = 0;
        field = targetObject.GetIterator();
        field.NextVisible(true);

        if (SHOW_SCRIPT_FIELD)
        {
            //Show the disabled script field
            EditorGUI.BeginDisabledGroup(true);
            EditorGUI.PropertyField(propertyRects[index], field, true);
            EditorGUI.EndDisabledGroup();
            index++;
        }

        //Replacement for "editor.OnInspectorGUI ();" so we have more control on how we draw the editor
        while (field.NextVisible(false))
        {
            try
            {
                EditorGUI.PropertyField(propertyRects[index], field, true);
            }
            catch (StackOverflowException)
            {
                field.objectReferenceValue = null;
                Debug.LogError("Detected self-nesting cauisng a StackOverflowException, avoid using the same " +
                    "object iside a nested structure.");
            }

            index++;
        }
        targetObject.ApplyModifiedProperties();

        EditorGUI.indentLevel--;
        #endregion
    }

    /// <summary>
    /// Draws the Background
    /// </summary>
    /// <param name="rect">The Rect where the background is drawn.</param>
    private void DrawBackground(Rect rect)
    {
        switch (BACKGROUND_STYLE)
        {

            case BackgroundStyles.HelpBox:
                EditorGUI.HelpBox(rect, "", MessageType.None);
                break;

            case BackgroundStyles.Darken:
                EditorGUI.DrawRect(rect, DARKEN_COLOUR);
                break;

            case BackgroundStyles.Lighten:
                EditorGUI.DrawRect(rect, LIGHTEN_COLOUR);
                break;
        }
    }


    private UnityEngine.Object CreateModule(UnityEngine.Object cloneObj,SerializedProperty prop)
    {
        var Clone = UnityEngine.Object.Instantiate(cloneObj);
        Clone.name = prop.serializedObject.targetObject.name +"_" +cloneObj.name;//將名稱前加上serializeObj的名稱，用於分類刪除
        UnityEngine.Object[] subObjs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(prop.serializedObject.targetObject));
        foreach (var sub in subObjs)//如果名稱相同
        {
            if (Clone.name == sub.name)
            {
                Clone.name += "(other)";
            }
        }
        AssetDatabase.AddObjectToAsset(Clone, prop.serializedObject.targetObject);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(prop.serializedObject.targetObject));
        return Clone;
    }
    private void DestroyModule(UnityEngine.Object lastObj,SerializedProperty prop) {
        if (lastObj != null) { 

            if (AssetDatabase.IsSubAsset(lastObj))
            {
                int splitIndex = lastObj.name.Split('_').Length;
                UnityEngine.Object[] subObjs = AssetDatabase.LoadAllAssetRepresentationsAtPath(AssetDatabase.GetAssetPath(prop.serializedObject.targetObject));//查找此資源底下的所有子資源
                for (int i = 0; i < subObjs.Length; i++)//把物件底下的子物件刪掉
                {
                    string[] subName = subObjs[i].name.Split('_');
                    if (subName.Length > splitIndex)
                    {
                        string subSerializeObjName = subName[0];
                        for (int j = 1; j < splitIndex; j++)
                        {
                            subSerializeObjName += "_" + subName[j];
                        }
                        if (subSerializeObjName == lastObj.name)
                        {
                            UnityEngine.Object.DestroyImmediate(subObjs[i], true);
                        }
                    }
                }
                UnityEngine.Object.DestroyImmediate(lastObj, true);//最後再刪除物件

                AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(prop.serializedObject.targetObject));

            }
        } 
    }
}
#endif