#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CustomEditor(typeof(AkRoom))]
public class AkRoomInspector : UnityEditor.Editor
{
	private readonly AkUnityEventHandlerInspector m_PostEventHandlerInspector = new AkUnityEventHandlerInspector();

	private AkRoom m_AkRoom;
	private UnityEditor.SerializedProperty priority;

	private UnityEditor.SerializedProperty reverbAuxBus;
	private UnityEditor.SerializedProperty reverbLevel;
	private UnityEditor.SerializedProperty wallOcclusion;
	private UnityEditor.SerializedProperty roomToneEvent;
	private UnityEditor.SerializedProperty roomToneAuxSend;

	private void OnEnable()
	{
		m_PostEventHandlerInspector.Init(serializedObject, "triggerList", "Trigger On: ", false);

		m_AkRoom = target as AkRoom;

		reverbAuxBus = serializedObject.FindProperty("reverbAuxBus");
		reverbLevel = serializedObject.FindProperty("reverbLevel");
		wallOcclusion = serializedObject.FindProperty("wallOcclusion");
		priority = serializedObject.FindProperty("priority");
		roomToneEvent = serializedObject.FindProperty("roomToneEvent");
		roomToneAuxSend = serializedObject.FindProperty("roomToneAuxSend");
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUILayout.PropertyField(reverbAuxBus);
			UnityEditor.EditorGUILayout.PropertyField(reverbLevel);
			UnityEditor.EditorGUILayout.PropertyField(wallOcclusion);
			UnityEditor.EditorGUILayout.PropertyField(priority);
		}

		UnityEditor.EditorGUILayout.LabelField("Room Tone", UnityEditor.EditorStyles.boldLabel);
		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			m_PostEventHandlerInspector.OnGUI();
			UnityEditor.EditorGUILayout.PropertyField(roomToneEvent);
			UnityEditor.EditorGUILayout.PropertyField(roomToneAuxSend);
		}

		AkGameObjectInspector.RigidbodyCheck(m_AkRoom.gameObject);

		serializedObject.ApplyModifiedProperties();
	}
}
#endif