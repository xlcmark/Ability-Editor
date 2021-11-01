using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ObjectSelectorWindow : EditorWindow
{
    static Object[] objs;
    static SerializedProperty prop;

    int selected=-1;

    GUIStyle style;

    public static void ShoWWindow(Object[] _objs,SerializedProperty _prop)
    {
        GetWindow<ObjectSelectorWindow>("ObjectSelector");
        objs = _objs;
        prop = _prop;
    }
    private void OnGUI()
    {
        if (objs==null) return;
        style = new GUIStyle();
        Texture2D texture = new Texture2D(2, 2);
        style.normal.background = texture;
        style.margin = new RectOffset(0, 0, 0, 0);

        for (int i = 0; i < objs.Length; i++)
        {
            if (selected == i)
            {
                style.normal.textColor = Color.white;
                SetColor(texture, new Color32(0, 140, 255,255));
            }
            else
            {
                style.normal.textColor = Color.black;
                SetColor(texture, Color.clear);
            }

            if (GUILayout.Button("    "+objs[i].name, style))
            {
                selected = i;
                PressButton();
            }
        }

    }
    private void PressButton()
    {
        prop.objectReferenceValue = objs[selected];
        prop.serializedObject.ApplyModifiedProperties();
    }

    private void SetColor(Texture2D tex2, Color32 color)
    {
        var fillColorArray = tex2.GetPixels32();

        for (var i = 0; i < fillColorArray.Length; ++i)
        {
            fillColorArray[i] = color;
        }

        tex2.SetPixels32(fillColorArray);

        tex2.Apply();
    }


}
