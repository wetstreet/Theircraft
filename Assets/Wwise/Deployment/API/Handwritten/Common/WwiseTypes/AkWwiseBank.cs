#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.

namespace AK.Wwise
{
	[System.Serializable]
	///@brief This type can be used to load/unload SoundBanks.
	public class Bank : BaseType
	{
		public WwiseBankReference WwiseObjectReference;

		public override WwiseObjectReference ObjectReference
		{
			get { return WwiseObjectReference; }
			set { WwiseObjectReference = value as WwiseBankReference; }
		}

		public override WwiseObjectType WwiseObjectType { get { return WwiseObjectType.Soundbank; } }

		public void Load(bool decodeBank = false, bool saveDecodedBank = false)
		{
			if (IsValid())
				AkBankManager.LoadBank(Name, decodeBank, saveDecodedBank);
		}

		public void LoadAsync(AkCallbackManager.BankCallback callback = null)
		{
			if (IsValid())
				AkBankManager.LoadBankAsync(Name, callback);
		}

		public void Unload()
		{
			if (IsValid())
				AkBankManager.UnloadBank(Name);
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.