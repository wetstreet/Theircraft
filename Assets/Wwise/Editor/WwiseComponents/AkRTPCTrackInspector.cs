#if UNITY_2017_1_OR_NEWER
[UnityEditor.CustomEditor(typeof(AkRTPCTrack))]
public class AkRTPCTrackInspector : UnityEditor.Editor
{
	private UnityEditor.SerializedProperty Parameter;

	public void OnEnable()
	{
		Parameter = serializedObject.FindProperty("Parameter");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUILayout.PropertyField(Parameter, new UnityEngine.GUIContent("Parameter: "));
		}

		serializedObject.ApplyModifiedProperties();
	}
}

#endif //UNITY_2017_1_OR_NEWER