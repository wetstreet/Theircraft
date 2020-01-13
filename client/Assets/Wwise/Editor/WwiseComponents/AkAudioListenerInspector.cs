#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AkAudioListener))]
public class AkAudioListenerInspector : UnityEditor.Editor
{
	private UnityEditor.SerializedProperty m_isDefaultListener;

	private void OnEnable()
	{
		m_isDefaultListener = serializedObject.FindProperty("isDefaultListener");
	}

	public override void OnInspectorGUI()
	{
		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUI.BeginChangeCheck();
			UnityEditor.EditorGUILayout.PropertyField(m_isDefaultListener);
			if (UnityEditor.EditorGUI.EndChangeCheck())
				serializedObject.ApplyModifiedProperties();
		}
	}
}
#endif