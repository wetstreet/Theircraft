#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public enum MultiPositionTypeLabel
{
	Simple_Mode,
	Large_Mode,
	MultiPosition_Mode
}

public class AkMultiPosEvent
{
	public bool eventIsPlaying;
	public System.Collections.Generic.List<AkAmbient> list = new System.Collections.Generic.List<AkAmbient>();

	public void FinishedPlaying(object in_cookie, AkCallbackType in_type, object in_info)
	{
		eventIsPlaying = false;
	}
}

[UnityEngine.AddComponentMenu("Wwise/AkAmbient")]
/// @brief Use this component to attach a Wwise Event to any object in a scene.
/// The sound can be started at various moments, dependent on the selected Unity trigger. This component is more useful for ambient sounds (sounds related to scene-bound objects) but could also be used for other purposes.
/// Since AkAmbient has AkEvent as its base class, it features the play/stop, play multiple, stop multiple and stop all buttons for previewing the associated Wwise event.
/// \sa
/// - \ref unity_use_AkEvent_AkAmbient
/// - \ref AkGameObj
/// - \ref AkEvent
/// - <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=soundengine__events.html" target="_blank">Integration Details - Events</a> (Note: This is described in the Wwise SDK documentation.)
public class AkAmbient : AkEvent
{
	public static System.Collections.Generic.Dictionary<uint, AkMultiPosEvent> multiPosEventTree =
		new System.Collections.Generic.Dictionary<uint, AkMultiPosEvent>();

	public AkMultiPositionType MultiPositionType = AkMultiPositionType.MultiPositionType_MultiSources;
	public MultiPositionTypeLabel multiPositionTypeLabel = MultiPositionTypeLabel.Simple_Mode;

	public AkAmbientLargeModePositioner[] LargeModePositions;

	private void OnEnable()
	{
		if (multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode)
		{
			var gameObj = gameObject.GetComponents<AkGameObj>();
			for (var i = 0; i < gameObj.Length; i++)
				gameObj[i].enabled = false;

			AkMultiPosEvent eventPosList;

			if (multiPosEventTree.TryGetValue(data.Id, out eventPosList))
			{
				if (!eventPosList.list.Contains(this))
					eventPosList.list.Add(this);
			}
			else
			{
				eventPosList = new AkMultiPosEvent();
				eventPosList.list.Add(this);
				multiPosEventTree.Add(data.Id, eventPosList);
			}

			var positionArray = BuildMultiDirectionArray(eventPosList);

			//Set multiple positions
			AkSoundEngine.SetMultiplePositions(eventPosList.list[0].gameObject, positionArray, (ushort) positionArray.Count,
				MultiPositionType);
		}
	}

	protected override void Start()
	{
		base.Start();

		if (multiPositionTypeLabel == MultiPositionTypeLabel.Simple_Mode)
		{
			var gameObj = gameObject.GetComponents<AkGameObj>();
			for (var i = 0; i < gameObj.Length; i++)
				gameObj[i].enabled = true;
		}
		else if (multiPositionTypeLabel == MultiPositionTypeLabel.Large_Mode)
		{
#if UNITY_EDITOR
			if (!UnityEditor.EditorApplication.isPlaying)
			{
				return;
			}
#endif
			var gameObj = gameObject.GetComponents<AkGameObj>();
			for (var i = 0; i < gameObj.Length; i++)
				gameObj[i].enabled = false;

			var positionArray = BuildAkPositionArray();
			AkSoundEngine.SetMultiplePositions(gameObject, positionArray, (ushort)positionArray.Count, MultiPositionType);
		}
	}

	private void OnDisable()
	{
		if (multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode)
		{
			var eventPosList = multiPosEventTree[data.Id];

			if (eventPosList.list.Count == 1)
				multiPosEventTree.Remove(data.Id);
			else
			{
				eventPosList.list.Remove(this);

				var positionArray = BuildMultiDirectionArray(eventPosList);
				AkSoundEngine.SetMultiplePositions(eventPosList.list[0].gameObject, positionArray, (ushort) positionArray.Count,
					MultiPositionType);
			}
		}
	}

	public override void HandleEvent(UnityEngine.GameObject in_gameObject)
	{
		if (multiPositionTypeLabel != MultiPositionTypeLabel.MultiPosition_Mode)
		{
			base.HandleEvent(in_gameObject);
		}
		else
		{
			var multiPositionSoundEmitter = multiPosEventTree[data.Id];
			if (multiPositionSoundEmitter.eventIsPlaying)
				return;

			multiPositionSoundEmitter.eventIsPlaying = true;

			soundEmitterObject = multiPositionSoundEmitter.list[0].gameObject;

			if (enableActionOnEvent)
				data.ExecuteAction(soundEmitterObject, actionOnEventType, (int)transitionDuration * 1000, curveInterpolation);
			else
				playingId = data.Post(soundEmitterObject, (uint)AkCallbackType.AK_EndOfEvent, multiPositionSoundEmitter.FinishedPlaying);
		}
	}

	public void OnDrawGizmosSelected()
	{
		UnityEngine.Gizmos.DrawIcon(transform.position, "WwiseAudioSpeaker.png", false);

#if UNITY_EDITOR
		if (multiPositionTypeLabel != MultiPositionTypeLabel.Large_Mode)
		{
			foreach (var entry in LargeModePositions)
			{
				if (entry != null)
				{
					UnityEngine.Gizmos.color = UnityEngine.Color.green;
					UnityEngine.Gizmos.DrawSphere(entry.transform.position, 0.1f);

					UnityEditor.Handles.Label(entry.transform.position, entry.name);
				}
			}
		}
#endif
	}

	public AkPositionArray BuildMultiDirectionArray(AkMultiPosEvent eventPosList)
	{
		var positionArray = new AkPositionArray((uint) eventPosList.list.Count);
		for (var i = 0; i < eventPosList.list.Count; i++)
		{
			positionArray.Add(eventPosList.list[i].transform.position, eventPosList.list[i].transform.forward,
				eventPosList.list[i].transform.up);
		}

		return positionArray;
	}

	private AkPositionArray BuildAkPositionArray()
	{
		var validPositionList = new System.Collections.Generic.List<AkAmbientLargeModePositioner>();

		for( int i= 0; i < LargeModePositions.Length; ++i)
		{
			if (LargeModePositions[i] != null)
			{
				if (!validPositionList.Contains(LargeModePositions[i]))
				{
					validPositionList.Add(LargeModePositions[i]);
				}
			}
		}

		var positionArray = new AkPositionArray((uint)validPositionList.Count);
		for (int i = 0; i < validPositionList.Count; ++i)
		{
			positionArray.Add(validPositionList[i].Position, validPositionList[i].Forward, validPositionList[i].Up);
		}

		return positionArray;
	}

	#region WwiseMigration
#pragma warning disable 0414 // private field assigned but not used.
	[UnityEngine.HideInInspector]
	[UnityEngine.SerializeField]
	public System.Collections.Generic.List<UnityEngine.Vector3> multiPositionArray = null;
#pragma warning restore 0414 // private field assigned but not used.

#if UNITY_EDITOR
	public override bool Migrate(UnityEditor.SerializedObject obj)
	{
		var hasMigrated = base.Migrate(obj);

		if (!AkUtilities.IsMigrationRequired(AkUtilities.MigrationStep.AkAmbient_v2019_1_0))
		{
			return hasMigrated;
		}

		var multiPositionTypeLabelProperty = obj.FindProperty("multiPositionTypeLabel");
		if (multiPositionTypeLabelProperty == null)
		{
			return hasMigrated;
		}

		if (multiPositionTypeLabelProperty.intValue != (int)MultiPositionTypeLabel.Large_Mode)
		{
			return hasMigrated;
		}

		var multiPositionArrayProperty = obj.FindProperty("multiPositionArray");
		if (multiPositionArrayProperty == null)
		{
			return hasMigrated;
		}

		if (multiPositionArrayProperty.arraySize == 0)
		{
			return hasMigrated;
		}

		var largeModePositionsProperty = obj.FindProperty("LargeModePositions");
		if (largeModePositionsProperty == null)
		{
			return hasMigrated;
		}

		largeModePositionsProperty.arraySize = multiPositionArrayProperty.arraySize;

		for (int point = 0; point < multiPositionArrayProperty.arraySize; ++point)
		{
			var elementProperty = multiPositionArrayProperty.GetArrayElementAtIndex(point);

			var largeModePositionElementProperty = largeModePositionsProperty.GetArrayElementAtIndex(point);
			if (largeModePositionElementProperty != null)
			{
				UnityEngine.GameObject newPoint = new UnityEngine.GameObject("AkAmbientPoint" + point.ToString());
				newPoint.AddComponent<AkAmbientLargeModePositioner>();
				newPoint.transform.SetParent(transform);
				newPoint.transform.position = transform.TransformPoint(elementProperty.vector3Value);

				largeModePositionElementProperty.objectReferenceValue = newPoint.GetComponent<AkAmbientLargeModePositioner>();
			}
		}

		multiPositionArrayProperty.arraySize = 0;
		return true;
	}
#endif
	#endregion
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.