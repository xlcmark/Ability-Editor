using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomPropertyDrawer(typeof(TestAttribute))]
public class TestAttributeDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {


     /*    Object[] objs= Resources.LoadAll("Module", typeof(BaseAction));
         string[] objNames = new string[objs.Length];
         for (int i = 0; i < objs.Length; i++)
         {
             objNames[i] = objs[i].ToString();
         }
         */

        // selected = EditorGUI.Popup(position, label.text, selected, objNames);
        // EditorGUI.LabelField( position,label.text,"aaaa" , EditorStyles.textField);

        /*    string[] path = property.propertyPath.Split('.');
            string ArrayName = path[0];//尋找陣列路徑(path)
            if (path.Length >= 2)
            {
                for (int i = 1; i < path.Length - 2; i++)//陣列為最下層往上兩層
                {
                    ArrayName += "." + path[i];
                }
            }
            SerializedProperty p = property.serializedObject.FindProperty(ArrayName);//得到陣列
            Rect rect1 = position;
            rect1.xMax = position.xMax - 50;
            property.intValue = EditorGUI.IntField(rect1, property.intValue);

            Rect rect2 = position;
            rect2.xMin = position.xMax - 50;
            if (GUI.Button(rect2, new GUIContent("Remove")))
            {
              //  Debug.Log(path[path.Length - 1].Substring(5, 1));
              //  int ArrayIndex = int.Parse(path[path.Length - 1].Substring(5, 1));
               // p.arraySize--;

                // p.DeleteArrayElementAtIndex(ArrayIndex);
                property.DeleteCommand();


            }

        */

        EditorGUI.LabelField(position, label);
        if (Event.current.type == UnityEngine.EventType.MouseDown)
        {
            if (position.Contains(Event.current.mousePosition))
            {

            }

        }

        //string dataPath = AssetDatabase.GetAssetPath(property.serializedObject.targetObject);
       // SerializedObject target = new SerializedObject(AssetDatabase.LoadAssetAtPath<Ability>(dataPath));

    }

}

