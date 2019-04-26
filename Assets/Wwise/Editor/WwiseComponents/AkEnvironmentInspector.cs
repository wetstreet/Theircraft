#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AkEnvironment))]
public class AkEnvironmentInspector : AkBaseInspector
{
	private AkEnvironment m_AkEnvironment;

	private UnityEditor.SerializedProperty m_excludeOthers;
	private UnityEditor.SerializedProperty m_isDefault;
	private UnityEditor.SerializedProperty m_priority;

	private void OnEnable()
	{
		m_AkEnvironment = target as AkEnvironment;

		m_priority = serializedObject.FindProperty("priority");
		m_isDefault = serializedObject.FindProperty("isDefault");
		m_excludeOthers = serializedObject.FindProperty("excludeOthers");

		//We move and replace the game object to trigger the OnTriggerStay function
		ShakeEnvironment();
	}

	public override void OnChildInspectorGUI()
	{
		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			m_priority.intValue = UnityEditor.EditorGUILayout.IntField("Priority: ", m_priority.intValue);

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

			m_isDefault.boolValue = UnityEditor.EditorGUILayout.Toggle("Default: ", m_isDefault.boolValue);
			if (m_isDefault.boolValue)
				m_excludeOthers.boolValue = false;

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

			m_excludeOthers.boolValue = UnityEditor.EditorGUILayout.Toggle("Exclude Others: ", m_excludeOthers.boolValue);
			if (m_excludeOthers.boolValue)
				m_isDefault.boolValue = false;
		}

		AkGameObjectInspector.RigidbodyCheck(m_AkEnvironment.gameObject);
	}

	public void ShakeEnvironment()
	{
		var temp = m_AkEnvironment.transform.position;
		temp.x *= 1.0000001f;
		m_AkEnvironment.transform.position = temp;

		UnityEditor.EditorApplication.update += ReplaceEnvironment;
	}

	private void ReplaceEnvironment()
	{
		UnityEditor.EditorApplication.update -= ReplaceEnvironment;
		if (m_AkEnvironment && m_AkEnvironment.transform)
		{
			var temp = m_AkEnvironment.transform.position;
			temp.x /= 1.0000001f;
			m_AkEnvironment.transform.position = temp;
		}
	}
}
#endif
