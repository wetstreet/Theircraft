#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type can be used to set Switch values on gameobjects.
	public class Switch : BaseGroupType
	{
		public WwiseSwitchReference WwiseObjectReference;

		public override WwiseObjectReference ObjectReference
		{
			get { return WwiseObjectReference; }
			set { WwiseObjectReference = value as WwiseSwitchReference; }
		}

		public override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Switch; } }
		public override WwiseObjectType WwiseObjectGroupType { get { return WwiseObjectType.SwitchGroup; } }

		public void SetValue(UnityEngine.GameObject gameObject)
		{
			if (IsValid())
			{
				var result = AkSoundEngine.SetSwitch(GroupId, Id, gameObject);
				Verify(result);
			}
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.