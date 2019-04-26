#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public abstract class AkBaseInspector : UnityEditor.Editor
{
	public abstract void OnChildInspectorGUI();

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		OnChildInspectorGUI();

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEngine.GUILayout.HorizontalScope("box"))
			UnityEditor.EditorGUILayout.PropertyField(serializedObject.FindProperty("data"), new UnityEngine.GUIContent("Name: "));

		serializedObject.ApplyModifiedProperties();

		if (UnityEngine.GUI.changed)
			UnityEditor.EditorUtility.SetDirty(serializedObject.targetObject);
	}
}
#endif
