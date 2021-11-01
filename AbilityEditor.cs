/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
[CustomEditor(typeof(Ability))]
public class AbilityEditor :  Editor{
	Ability ability;
	Editor compomentEditor;

	void OnEnable(){
		ability = target as Ability;
	}
	public override void OnInspectorGUI ()
	{
		EditorGUI.BeginChangeCheck ();
		EditorGUILayout.LabelField ("Desciption");
		var desciption = EditorGUILayout.TextArea (ability.Desciption, GUILayout.MinHeight (100));
		if (EditorGUI.EndChangeCheck ()) {
			Undo.RecordObject (ability, "Change Desciption");

			ability.Desciption = desciption;
			EditorUtility.SetDirty (ability);
		}

		EditorGUILayout.Space ();

		ability.action = ModuleField ("Action", ability.action);

	}
	private T ModuleField<T>(string label,T current)where T :module{
		T result;
		EditorGUI.BeginChangeCheck ();
		var module = (T)EditorGUILayout.ObjectField (label, current, typeof(T), false);
		if (EditorGUI.EndChangeCheck ()) {
			if (module != null && module != current) {
				if (DestroyExistingModule (current)) {
					result = CreateModuleFromTemplate (module);
				} else
					result = current;
			} else {
				if (DestroyExistingModule (current))
					result = null;
				else
					result = current;
			}				
		} else
			result = current;

		if (result != null) {
			
			CreateCachedEditor (result, null, ref compomentEditor);
			GUILayout.BeginVertical (EditorStyles.helpBox);
			compomentEditor.OnInspectorGUI ();
			GUILayout.EndVertical ();
		}
		return result;
	}
	private bool DestroyExistingModule(module md){
		if (md != null) {
			if (AssetDatabase.IsSubAsset (md)) {
				if (EditorUtility.DisplayDialog ("刪除膜塊", "Are you sure you want to remove the existing module? This operation cannot be undone.", "Continue", "Cancel")) {
					DestroyImmediate (md);
					AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (ability));
					EditorUtility.SetDirty (ability);
					return true;
				} else
					return false;
			}
		}
		return true;
	}
	private T CreateModuleFromTemplate<T>(T module)where T:module{
		var clone = Instantiate (module);
		clone.name=clone.name.Replace("(Clone)","");
		Undo.RegisterCreatedObjectUndo (clone, "Add Component");
		AssetDatabase.AddObjectToAsset (clone, ability);
		AssetDatabase.ImportAsset (AssetDatabase.GetAssetPath (ability));

		EditorUtility.SetDirty (ability);
		return clone;
	}
}
*/