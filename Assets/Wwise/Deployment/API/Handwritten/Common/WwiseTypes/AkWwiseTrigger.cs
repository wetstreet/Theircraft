#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type can be used to post triggers to the sound engine.
	public class Trigger : BaseType
	{
		public WwiseTriggerReference WwiseObjectReference;

		public override WwiseObjectReference ObjectReference
		{
			get { return WwiseObjectReference; }
			set { WwiseObjectReference = value as WwiseTriggerReference; }
		}

		public override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Trigger; } }

		public void Post(UnityEngine.GameObject gameObject)
		{
			if (IsValid())
			{
				var result = AkSoundEngine.PostTrigger(Id, gameObject);
				Verify(result);
			}
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.