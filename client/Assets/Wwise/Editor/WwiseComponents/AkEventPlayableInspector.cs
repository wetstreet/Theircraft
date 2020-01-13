#if UNITY_EDITOR && UNITY_2017_1_OR_NEWER

//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////
[UnityEditor.CustomEditor(typeof(AkEventPlayable))]
public class AkEventPlayableInspector : UnityEditor.Editor
{
	private UnityEditor.SerializedProperty akEvent;
	private UnityEditor.SerializedProperty emitterObjectRef;
	private AkEventPlayable m_AkEventPlayable;

	private UnityEditor.SerializedProperty overrideTrackEmitterObject;
	private UnityEditor.SerializedProperty retriggerEvent;

	public void OnEnable()
	{
		m_AkEventPlayable = target as AkEventPlayable;
		akEvent = serializedObject.FindProperty("akEvent");
		overrideTrackEmitterObject = serializedObject.FindProperty("overrideTrackEmitterObject");
		emitterObjectRef = serializedObject.FindProperty("emitterObjectRef");
		retriggerEvent = serializedObject.FindProperty("retriggerEvent");

		AkWwiseXMLWatcher.Instance.XMLUpdated += updateClipMaxDuration;

		if (m_AkEventPlayable != null)
		{
			m_AkEventPlayable.EditorValidated += updateClipMaxDuration;

			updateClipMaxDuration();
		}
	}

	public void OnDisable()
	{
		AkWwiseXMLWatcher.Instance.XMLUpdated -= updateClipMaxDuration;

		if (m_AkEventPlayable != null)
		{
			m_AkEventPlayable.EditorValidated -= updateClipMaxDuration;
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUILayout.PropertyField(overrideTrackEmitterObject, new UnityEngine.GUIContent("Override Track Object: "));

			if (overrideTrackEmitterObject.boolValue)
			{
				UnityEditor.EditorGUILayout.PropertyField(emitterObjectRef, new UnityEngine.GUIContent("Emitter Object Ref: "));
			}

			UnityEditor.EditorGUILayout.PropertyField(retriggerEvent, new UnityEngine.GUIContent("Retrigger Event: "));

			UnityEditor.EditorGUILayout.PropertyField(akEvent, new UnityEngine.GUIContent("Event: "));
		}

		if (m_AkEventPlayable != null && m_AkEventPlayable.OwningClip != null)
		{
			m_AkEventPlayable.OwningClip.displayName = m_AkEventPlayable.akEvent.Name;
		}

		serializedObject.ApplyModifiedProperties();
	}

	private void updateClipMaxDuration()
	{
		if (m_AkEventPlayable != null)
		{
			var newMinMaxDuration = getMinMaxDuration(m_AkEventPlayable.akEvent);

			if (newMinMaxDuration != UnityEngine.Vector2.zero)
			{
				m_AkEventPlayable.EventDurationMin = newMinMaxDuration.x;
				m_AkEventPlayable.EventDurationMax = newMinMaxDuration.y;

				if (m_AkEventPlayable.OwningClip != null)
				{
					m_AkEventPlayable.OwningClip.duration = m_AkEventPlayable.EventDurationMax;
				}
			}
		}
	}

	private UnityEngine.Vector2 getMinMaxDuration(AK.Wwise.Event akEvent)
	{
		UnityEngine.Vector2 result = UnityEngine.Vector2.zero;

		var eventInfo = AkWwiseProjectInfo.GetData().GetEventInfo(akEvent.Id);
		if (eventInfo != null)
		{
			result.x = eventInfo.minDuration;
			result.y = eventInfo.maxDuration;
		}

		return result;
	}
}

#endif //#if UNITY_EDITOR && UNITY_2017_1_OR_NEWER