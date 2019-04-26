#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

#if UNITY_2017_1_OR_NEWER
[UnityEngine.Timeline.TrackColor(0.32f, 0.13f, 0.13f)]
// Specifies the type of Playable Asset this track manages
[UnityEngine.Timeline.TrackClipType(typeof(AkRTPCPlayable))]
// Use if the track requires a binding to a scene object or asset
[UnityEngine.Timeline.TrackBindingType(typeof(UnityEngine.GameObject))]
public class AkRTPCTrack : UnityEngine.Timeline.TrackAsset
{
	public AK.Wwise.RTPC Parameter;

	// override the type of mixer playable used by this track
	public override UnityEngine.Playables.Playable CreateTrackMixer(UnityEngine.Playables.PlayableGraph graph,
		UnityEngine.GameObject go, int inputCount)
	{
		var playable = UnityEngine.Playables.ScriptPlayable<AkRTPCPlayableBehaviour>.Create(graph, inputCount);
		setPlayableProperties();
		return playable;
	}

	public void setPlayableProperties()
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			var clipPlayable = (AkRTPCPlayable) clip.asset;
			clipPlayable.Parameter = Parameter;
			clipPlayable.OwningClip = clip;
		}
	}

	public void OnValidate()
	{
		var clips = GetClips();
		foreach (var clip in clips)
		{
			var clipPlayable = (AkRTPCPlayable) clip.asset;
			clipPlayable.Parameter = Parameter;
		}
	}
}

#endif //UNITY_2017_1_OR_NEWER

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.