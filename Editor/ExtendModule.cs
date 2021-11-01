/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
//舊版本
//[CustomPropertyDrawer(typeof(Module), true)]
public class ExtendModule : PropertyDrawer
{
    private float INNER_SPACING=6.0f;
    private float OUTER_SPACING=4.0f;


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
        fieldRect.height = EditorGUIUtility.singleLineHeight;//固定高度

             if (property.serializedObject.targetObject is ScriptableObject)
             {
                 #region 選擇一個模塊並複製
                 EditorGUI.BeginChangeCheck();
                 var module = (Module)EditorGUI.ObjectField(fieldRect, label, property.objectReferenceValue, fieldInfo.FieldType, false);
                 if (EditorGUI.EndChangeCheck())
                 {
                     if (module != null && module != property.objectReferenceValue)
                     {
                         if (DestroyExisingModule(property))
                         {
                             property.objectReferenceValue = CreateModule(module, property);
                         }
                     }
                     else
                     {
                         DestroyExisingModule(property);
                     }

                 }

                 #endregion
             }
             else//本體是Mono時不顯示
             {
              EditorGUI.PropertyField(fieldRect, property, label, true);
              return;
             }



    //    EditorGUI.PropertyField(fieldRect, property, label, true);

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



        while (field.NextVisible(false))
        {
            marchingRect.y += marchingRect.height + EditorGUIUtility.standardVerticalSpacing;
            marchingRect.height = EditorGUI.GetPropertyHeight(field, true);
            propertyRects.Add(marchingRect);
        }

        marchingRect.y += INNER_SPACING;

        bodyRect.yMax = marchingRect.yMax;
        #endregion



        EditorGUI.HelpBox(bodyRect, "", MessageType.None);

        #region Draw Fields
        EditorGUI.indentLevel++;

        int index = 0;
        field = targetObject.GetIterator();
        field.NextVisible(true);

       

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

        private Module CreateModule(Module module, SerializedProperty property)
    {
        var clone = (Module)UnityEngine.Object.Instantiate(module);
        AssetDatabase.AddObjectToAsset(clone, property.serializedObject.targetObject);
        AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
        return clone;
        

    }
    private bool DestroyExisingModule(SerializedProperty property)
    {
        if (property.objectReferenceValue != null)
        {
            if (AssetDatabase.IsSubAsset(property.objectReferenceValue))
            {
                if (EditorUtility.DisplayDialog("刪除膜塊", "Are you sure you want to remove the existing module? This operation cannot be undone.", "Continue", "Cancel"))
                {
                    UnityEngine.Object.DestroyImmediate(property.objectReferenceValue,true);
                    AssetDatabase.ImportAsset(AssetDatabase.GetAssetPath(property.serializedObject.targetObject));
                    return true;
                }
                else
                    return false;
            }
        }
        return true;
    }

}
*/
