#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type represents an Acoustic Texture.
	public class AcousticTexture : BaseType
	{
		public WwiseAcousticTextureReference WwiseObjectReference;

		public override WwiseObjectReference ObjectReference
		{
			get { return WwiseObjectReference; }
			set { WwiseObjectReference = value as WwiseAcousticTextureReference; }
		}

		public override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.AcousticTexture; } }
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.