#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

#if UNITY_2017_1_OR_NEWER
//--------------------------------------------------------------------------------------------
// The representation of the Timeline Clip
//--------------------------------------------------------------------------------------------

[System.Serializable]
public class AkRTPCPlayable : UnityEngine.Playables.PlayableAsset, UnityEngine.Timeline.ITimelineClipAsset
{
	public bool overrideTrackObject = false;
	public UnityEngine.ExposedReference<UnityEngine.GameObject> RTPCObject;

	public bool setRTPCGlobally = false;
	public AkRTPCPlayableBehaviour template = new AkRTPCPlayableBehaviour();

	public AK.Wwise.RTPC Parameter { get; set; }

	public UnityEngine.Timeline.TimelineClip OwningClip { get; set; }

	public UnityEngine.Timeline.ClipCaps clipCaps
	{
		get
		{
			return UnityEngine.Timeline.ClipCaps.Looping & UnityEngine.Timeline.ClipCaps.Extrapolation &
			       UnityEngine.Timeline.ClipCaps.ClipIn & UnityEngine.Timeline.ClipCaps.SpeedMultiplier;
		}
	}

	public override UnityEngine.Playables.Playable CreatePlayable(UnityEngine.Playables.PlayableGraph graph,
		UnityEngine.GameObject go)
	{
		var playable = UnityEngine.Playables.ScriptPlayable<AkRTPCPlayableBehaviour>.Create(graph, template);
		var b = playable.GetBehaviour();
		InitializeBehavior(graph, ref b, go);
		return playable;
	}

	public void InitializeBehavior(UnityEngine.Playables.PlayableGraph graph, ref AkRTPCPlayableBehaviour b,
		UnityEngine.GameObject owner)
	{
		b.overrideTrackObject = overrideTrackObject;
		b.setRTPCGlobally = setRTPCGlobally;

		if (overrideTrackObject)
			b.rtpcObject = RTPCObject.Resolve(graph.GetResolver());
		else
			b.rtpcObject = owner;

		b.parameter = Parameter;
	}
}


//--------------------------------------------------------------------------------------------
// The behaviour template.
//--------------------------------------------------------------------------------------------

[System.Serializable]
public class AkRTPCPlayableBehaviour : UnityEngine.Playables.PlayableBehaviour
{
	private bool m_OverrideTrackObject;

	private AK.Wwise.RTPC m_Parameter;
	private UnityEngine.GameObject m_RTPCObject;
	private bool m_SetRTPCGlobally;

	public float RTPCValue = 0.0f;

	public bool setRTPCGlobally
	{
		set { m_SetRTPCGlobally = value; }
	}

	public bool overrideTrackObject
	{
		set { m_OverrideTrackObject = value; }
	}

	public UnityEngine.GameObject rtpcObject
	{
		set { m_RTPCObject = value; }
		get { return m_RTPCObject; }
	}

	public AK.Wwise.RTPC parameter
	{
		set { m_Parameter = value; }
	}

	public override void ProcessFrame(UnityEngine.Playables.Playable playable, UnityEngine.Playables.FrameData info,
		object playerData)
	{
		if (!m_OverrideTrackObject)
		{
			// At this point, m_RTPCObject will have been set to the timeline owner object in AkRTPCPlayable::CreatePlayable().
			// If the track object is null, we keep using the timeline owner object. Otherwise, we swap it for the track object.
			var obj = playerData as UnityEngine.GameObject;
			if (obj != null) m_RTPCObject = obj;
		} //If we are overriding the track object, the m_RTPCObject will have been resolved in AkRTPCPlayable::CreatePlayable().

		if (m_Parameter != null)
		{
			if (m_SetRTPCGlobally || m_RTPCObject == null)
				m_Parameter.SetGlobalValue(RTPCValue);
			else
				m_Parameter.SetValue(m_RTPCObject, RTPCValue);
		}
	}
}

#endif //UNITY_2017_1_OR_NEWER

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.