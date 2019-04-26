#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type can be used to set Wwise States.
	public class State : BaseGroupType
	{
		public WwiseStateReference WwiseObjectReference;

		public override WwiseObjectReference ObjectReference
		{
			get { return WwiseObjectReference; }
			set { WwiseObjectReference = value as WwiseStateReference; }
		}

		public override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.State; } }
		public override WwiseObjectType WwiseObjectGroupType { get { return WwiseObjectType.StateGroup; } }

		public void SetValue()
		{
			if (IsValid())
			{
				var result = AkSoundEngine.SetState(GroupId, Id);
				Verify(result);
			}
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.