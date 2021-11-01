
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
//[CustomEditor(typeof(Ability))]
[CanEditMultipleObjects]
public class AbilityEditor : Editor
{
    private Ability ae;
    private ReorderableList list;

    private void OnEnable()
    {
        ae = (Ability)target;
        list = new ReorderableList(serializedObject, serializedObject.FindProperty("ms"), true, true, true, true);
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        serializedObject.Update();
        list.DoLayoutList();
        serializedObject.ApplyModifiedProperties();
        EditorUtility.SetDirty(target);
    }
}
