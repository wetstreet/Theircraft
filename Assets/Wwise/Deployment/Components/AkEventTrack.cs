#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

#if UNITY_2017_1_OR_NEWER

//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEngine.Timeline.TrackColor(0.855f, 0.8623f, 0.870f)]
[UnityEngine.Timeline.TrackClipType(typeof(AkEventPlayable))]
[UnityEngine.Timeline.TrackBindingType(typeof(UnityEngine.GameObject))]
/// @brief A track within timeline that holds \ref AkEventPlayable clips. 
/// @details AkEventTracks are bound to a specific GameObject, which is the default emitter for all of the \ref AkEventPlayable clips. There is an option to override this in /ref AkEventPlayable.
/// \sa
/// - \ref AkEventPlayable
/// - \ref AkEventPlayableBehavior
public class AkEventTrack : UnityEngine.Timeline.TrackAsset
{
	public override UnityEngine.Playables.Playable CreateTrackMixer(UnityEngine.Playables.PlayableGraph graph,
		UnityEngine.GameObject go, int inputCount)
	{
#if UNITY_EDITOR
		var Settings = WwiseSettings.LoadSettings();
		var WprojPath = AkUtilities.GetFullPath(UnityEngine.Application.dataPath, Settings.WwiseProjectPath);
		AkUtilities.EnableBoolSoundbankSettingInWproj("SoundBankGenerateEstimatedDuration", WprojPath);
#endif
		var playable = UnityEngine.Playables.ScriptPlayable<AkEventPlayableBehavior>.Create(graph);
		UnityEngine.Playables.PlayableExtensions.SetInputCount(playable, inputCount);
		setFadeTimes();
		setOwnerClips();
		return playable;
	}

#if UNITY_EDITOR
	public void Awake()
	{
		AkWwiseXMLWatcher.Instance.XMLUpdated += OnXMLUpdated;
	}

	public void OnDestroy()
	{
		AkWwiseXMLWatcher.Instance.XMLUpdated -= OnXMLUpdated;
	}
#endif

	public void setFadeTimes()
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			var clipPlayable = (AkEventPlayable) clip.asset;
			clipPlayable.setBlendInDuration((float) getBlendInTime(clipPlayable));
			clipPlayable.setBlendOutDuration((float) getBlendOutTime(clipPlayable));
			clipPlayable.setEaseInDuration((float) getEaseInTime(clipPlayable));
			clipPlayable.setEaseOutDuration((float) getEaseOutTime(clipPlayable));
		}
	}

	public void setOwnerClips()
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			var clipPlayable = (AkEventPlayable) clip.asset;
			clipPlayable.OwningClip = clip;
		}
	}

	public double getBlendInTime(AkEventPlayable playableClip)
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			if (playableClip == (AkEventPlayable) clip.asset)
				return clip.blendInDuration;
		}

		return 0.0;
	}

	public double getBlendOutTime(AkEventPlayable playableClip)
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			if (playableClip == (AkEventPlayable) clip.asset)
				return clip.blendOutDuration;
		}

		return 0.0;
	}

	public double getEaseInTime(AkEventPlayable playableClip)
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			if (playableClip == (AkEventPlayable) clip.asset)
				return clip.easeInDuration;
		}

		return 0.0;
	}

	public double getEaseOutTime(AkEventPlayable playableClip)
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			if (playableClip == (AkEventPlayable) clip.asset)
				return clip.easeOutDuration;
		}

		return 0.0;
	}

#if UNITY_EDITOR
	private void OnXMLUpdated()
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			// The setter of OwningClip call Refresh() in Editor mode
			((AkEventPlayable)clip.asset).OwningClip = clip;
		}
	}
#endif
}

#endif //UNITY_2017_1_OR_NEWER
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.