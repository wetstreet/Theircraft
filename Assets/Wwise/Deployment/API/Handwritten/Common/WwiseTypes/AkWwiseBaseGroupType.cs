#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type represents the base for all Wwise Types that also require a group GUID, such as State and Switch.
	public abstract class BaseGroupType : BaseType
	{
		public WwiseObjectReference GroupWwiseObjectReference
		{
			get
			{
				var reference = ObjectReference as WwiseGroupValueObjectReference;
				return reference ? reference.GroupObjectReference : null;
			}
		}

		public abstract WwiseObjectType WwiseObjectGroupType { get; }

		public uint GroupId
		{
			get { return GroupWwiseObjectReference ? GroupWwiseObjectReference.Id : AkSoundEngine.AK_INVALID_UNIQUE_ID; }
		}

		public override bool IsValid()
		{
			return base.IsValid() && GroupWwiseObjectReference != null;
		}

		#region Obsolete
		[System.Obsolete(AkSoundEngine.Deprecation_2018_1_2)]
		public int groupID
		{
			get { return (int)GroupId; }
		}

		[System.Obsolete(AkSoundEngine.Deprecation_2018_1_6)]
		public byte[] groupGuid
		{
			get
			{
				var objRef = ObjectReference;
				return !objRef ? null : objRef.Guid.ToByteArray();
			}
		}
		#endregion

		#region WwiseMigration
#pragma warning disable 0414 // private field assigned but not used.
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("groupID")]
		private int groupIdInternal;
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("groupGuid")]
		private byte[] groupGuidInternal;
#pragma warning restore 0414 // private field assigned but not used.
		#endregion
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.