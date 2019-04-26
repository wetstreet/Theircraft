#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type represents the base for all Wwise Types that require a GUID.
	public abstract class BaseType
	{
		public abstract WwiseObjectReference ObjectReference { get; set; }

		public abstract WwiseObjectType WwiseObjectType { get; }

		public virtual string Name
		{
			get { return IsValid() ? ObjectReference.DisplayName : string.Empty; }
		}

		public uint Id
		{
			get { return IsValid() ? ObjectReference.Id : AkSoundEngine.AK_INVALID_UNIQUE_ID; }
		}

		public virtual bool IsValid()
		{
			return ObjectReference != null;
		}

		public bool Validate()
		{
			if (IsValid())
				return true;

			UnityEngine.Debug.LogWarning("Wwise ID has not been resolved. Consider picking a new " + GetType().Name + ".");
			return false;
		}

		protected void Verify(AKRESULT result)
		{
#if UNITY_EDITOR
			if (result != AKRESULT.AK_Success && AkSoundEngine.IsInitialized())
				UnityEngine.Debug.LogWarning("Unsuccessful call made on " + GetType().Name + ".");
#endif
		}

		public override string ToString()
		{
			return IsValid() ? ObjectReference.ObjectName : ("Empty " + GetType().Name);
		}

#if UNITY_EDITOR
		public void SetupReference(string name, System.Guid guid)
		{
			ObjectReference = WwiseObjectReference.FindOrCreateWwiseObject(WwiseObjectType, name, guid);
		}
#endif

		#region Obsolete
		[System.Obsolete(AkSoundEngine.Deprecation_2018_1_2)]
		public int ID
		{
			get { return (int)Id; }
		}

		[System.Obsolete(AkSoundEngine.Deprecation_2018_1_6)]
		public byte[] valueGuid
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
		[UnityEngine.Serialization.FormerlySerializedAs("ID")]
		private int idInternal;
		[UnityEngine.HideInInspector]
		[UnityEngine.SerializeField]
		[UnityEngine.Serialization.FormerlySerializedAs("valueGuid")]
		private byte[] valueGuidInternal;
#pragma warning restore 0414 // private field assigned but not used.
		#endregion
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.