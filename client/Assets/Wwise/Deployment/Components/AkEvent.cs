#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// <summary>
///     Event callback information.
///     Event callback functions can receive this structure as a parameter
/// </summary>
public class AkEventCallbackMsg
{
	///For more information about the event callback, see the classes derived from AkCallbackInfo.
	public AkCallbackInfo info;

	/// GameObject from whom the callback function was called
	public UnityEngine.GameObject sender;

	///AkSoundEngine.PostEvent callback flags. See the AkCallbackType enumeration for a list of all callbacks
	public AkCallbackType type;
}

[UnityEngine.AddComponentMenu("Wwise/AkEvent")]
[UnityEngine.RequireComponent(typeof(AkGameObj))]
/// @brief Helper class that knows a Wwise Event and when to trigger it in Unity. As of 2017.2.0, the AkEvent inspector has buttons for play/stop, play multiple, stop multiple, and stop all.
/// Play/Stop will play or stop the event such that it can be previewed both in edit mode and play mode. When multiple objects are selected, Play Multiple and Stop Multiple will play or stop the associated AkEvent for each object.
/// \sa
/// - \ref sect_edit_mode
/// - \ref unity_use_AkEvent_AkAmbient
/// - <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=soundengine__events.html" target="_blank">Integration Details - Events</a> (Note: This is described in the Wwise SDK documentation.)
public class AkEvent : AkDragDropTriggerHandler
#if UNITY_EDITOR
	, AK.Wwise.IMigratable
#endif
{
	/// Replacement action.  See AK::SoundEngine::ExecuteEventOnAction()
	public AkActionOnEventType actionOnEventType = AkActionOnEventType.AkActionOnEventType_Stop;

	/// Fade curve to use with the new Action.  See AK::SoundEngine::ExecuteEventOnAction()
	public AkCurveInterpolation curveInterpolation = AkCurveInterpolation.AkCurveInterpolation_Linear;

	/// Enables additional options to reuse existing events.  Use it to transform a Play event into a Stop event without having to define one in the Wwise Project.
	public bool enableActionOnEvent = false;

	public AK.Wwise.Event data = new AK.Wwise.Event();
	protected override AK.Wwise.BaseType WwiseType { get { return data; } }

	[System.Serializable]
	public class CallbackData
	{
		public AK.Wwise.CallbackFlags Flags;
		public string FunctionName;
		public UnityEngine.GameObject GameObject;

		public void CallFunction(AkEventCallbackMsg eventCallbackMsg)
		{
			if (((uint)eventCallbackMsg.type & Flags.value) != 0 && GameObject)
				GameObject.SendMessage(FunctionName, eventCallbackMsg);
		}
	}

	public bool useCallbacks = false;
	public System.Collections.Generic.List<CallbackData> Callbacks = new System.Collections.Generic.List<CallbackData>();

	public uint playingId = AkSoundEngine.AK_INVALID_PLAYING_ID;

	/// Game object onto which the Event will be posted.  By default, when empty, it is posted on the same object on which the component was added.
	public UnityEngine.GameObject soundEmitterObject;

	/// Duration of the fade.  See AK::SoundEngine::ExecuteEventOnAction()
	public float transitionDuration = 0.0f;

	private AkEventCallbackMsg EventCallbackMsg = null;

	protected override void Start()
	{
#if UNITY_EDITOR
		if (UnityEditor.BuildPipeline.isBuildingPlayer || AkUtilities.IsMigrating || !UnityEditor.EditorApplication.isPlaying)
		{
			base.Start();
			return;
		}
#endif

		if (useCallbacks)
			EventCallbackMsg = new AkEventCallbackMsg { sender = gameObject };

		base.Start();
	}

	private void Callback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info)
	{
		EventCallbackMsg.type = in_type;
		EventCallbackMsg.info = in_info;

		for (var i = 0; i < Callbacks.Count; ++i)
			Callbacks[i].CallFunction(EventCallbackMsg);
	}

	public override void HandleEvent(UnityEngine.GameObject in_gameObject)
	{
		var gameObj = useOtherObject && in_gameObject != null ? in_gameObject : gameObject;
		soundEmitterObject = gameObj;

		if (enableActionOnEvent)
		{
			data.ExecuteAction(gameObj, actionOnEventType, (int)transitionDuration * 1000, curveInterpolation);
			return;
		}

		if (useCallbacks)
		{
			uint flags = 0;
			for (var i = 0; i < Callbacks.Count; ++i)
			{
				if (Callbacks[i].GameObject && !string.IsNullOrEmpty(Callbacks[i].FunctionName))
					flags |= Callbacks[i].Flags.value;
			}

			if (flags != 0)
			{
				playingId = data.Post(gameObj, flags, Callback);
				return;
			}
		}

		playingId = data.Post(gameObj);
	}

	public void Stop(int _transitionDuration)
	{
		Stop(_transitionDuration, AkCurveInterpolation.AkCurveInterpolation_Linear);
	}

	public void Stop(int _transitionDuration, AkCurveInterpolation _curveInterpolation)
	{
		data.Stop(soundEmitterObject, _transitionDuration, _curveInterpolation);
	}

	#region Obsolete
	[System.Obsolete(AkSoundEngine.Deprecation_2018_1_2)]
	public int eventID { get { return (int)(data == null ? AkSoundEngine.AK_INVALID_UNIQUE_ID : data.Id); } }

	[System.Obsolete(AkSoundEngine.Deprecation_2018_1_6)]
	public byte[] valueGuid
	{
		get
		{
			if (data == null)
				return null;

			var objRef = data.ObjectReference;
			return !objRef ? null : objRef.Guid.ToByteArray();
		}
	}

	[System.Obsolete(AkSoundEngine.Deprecation_2018_1_6)]
	public AkEventCallbackData m_callbackData { get { return m_callbackDataInternal; } }
	#endregion

	#region WwiseMigration
#pragma warning disable 0414 // private field assigned but not used.
	[UnityEngine.HideInInspector]
	[UnityEngine.SerializeField]
	[UnityEngine.Serialization.FormerlySerializedAs("eventID")]
	private int eventIdInternal = (int)AkSoundEngine.AK_INVALID_UNIQUE_ID;
	[UnityEngine.HideInInspector]
	[UnityEngine.SerializeField]
	[UnityEngine.Serialization.FormerlySerializedAs("valueGuid")]
	private byte[] valueGuidInternal;
	[UnityEngine.HideInInspector]
	[UnityEngine.SerializeField]
	[UnityEngine.Serialization.FormerlySerializedAs("m_callbackData")]
	private AkEventCallbackData m_callbackDataInternal = null;
#pragma warning restore 0414 // private field assigned but not used.

#if UNITY_EDITOR
	public virtual bool Migrate(UnityEditor.SerializedObject obj)
	{
		var hasMigrated = false;
		if (AkUtilities.IsMigrationRequired(AkUtilities.MigrationStep.WwiseTypes_v2018_1_6))
		{
			hasMigrated = AK.Wwise.TypeMigration.ProcessSingleGuidType(obj.FindProperty("data.WwiseObjectReference"), WwiseObjectType.Event,
				obj.FindProperty("valueGuidInternal"), obj.FindProperty("eventIdInternal"));
		}

		if (!AkUtilities.IsMigrationRequired(AkUtilities.MigrationStep.AkEventCallback_v2018_1_6))
			return hasMigrated;

		var oldCallbackDataProperty = obj.FindProperty("m_callbackDataInternal");
		var oldCallbackData = oldCallbackDataProperty.objectReferenceValue as AkEventCallbackData;
		if (!oldCallbackData)
			return hasMigrated;

		var count = oldCallbackData.callbackFlags.Count;
		if (count != oldCallbackData.callbackFunc.Count || count != oldCallbackData.callbackGameObj.Count)
		{
			UnityEngine.Debug.LogWarning("WwiseUnity: Inconsistent callback data!");
			return hasMigrated;
		}

		var newCallbackData = obj.FindProperty("Callbacks");
		newCallbackData.arraySize = count;
		obj.FindProperty("useCallbacks").boolValue = true;

		for (var i = 0; i < count; ++i)
		{
			var data = newCallbackData.GetArrayElementAtIndex(i);
			data.FindPropertyRelative("GameObject").objectReferenceValue = oldCallbackData.callbackGameObj[i];
			data.FindPropertyRelative("FunctionName").stringValue = oldCallbackData.callbackFunc[i];
			data.FindPropertyRelative("Flags.value").intValue = oldCallbackData.callbackFlags[i];
			UnityEngine.Debug.Log("WwiseUnity: Migrated Callback for function \"" + oldCallbackData.callbackFunc[i] + "\" on <" + oldCallbackData.callbackGameObj[i] + "> with flags <" + (AkCallbackType)oldCallbackData.callbackFlags[i] + ">.");
		}

		return true;
	}
#endif
	#endregion
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.