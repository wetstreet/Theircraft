public class AkWindowsSettings : AkWwiseInitializationSettings.PlatformSettings
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticPlatformRegistration()
	{
		RegisterPlatformSettingsClass<AkWindowsSettings>("Windows");
	}
#endif // UNITY_EDITOR

	protected override AkCommonUserSettings GetUserSettings()
	{
		return UserSettings;
	}

	protected override AkCommonAdvancedSettings GetAdvancedSettings()
	{
		return AdvancedSettings;
	}

	protected override AkCommonCommSettings GetCommsSettings()
	{
		return CommsSettings;
	}

	[System.Serializable]
	public class PlatformAdvancedSettings : AkCommonAdvancedSettings
	{
		public enum AudioAPI
		{
			None = 0,
			Wasapi = 1 << 0,
			XAudio2 = 1 << 1,
			DirectSound = 1 << 2,
			Default = ~0,
		}

		[UnityEngine.Tooltip("Main audio API to use. Leave set to \"Default\" for the default audio sink. This value will be ignored if a valid \"AudioDeviceShareset\" is provided.")]
		[AkEnumFlag(typeof(AudioAPI))]
		public AudioAPI m_AudioAPI = AudioAPI.Default;

		[UnityEngine.Tooltip("Only used when \"AudioAPI\" is \"DirectSound\", sounds will be muted if set to false when the game loses the focus.")]
		public bool m_GlobalFocus = true;

		public override void CopyTo(AkPlatformInitSettings settings)
		{
#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN || UNITY_WSA
			settings.eAudioAPI = (AkAudioAPI)m_AudioAPI;
			settings.bGlobalFocus = m_GlobalFocus;
#endif
		}
	}

	[UnityEngine.HideInInspector]
	public AkCommonUserSettings UserSettings;

	[UnityEngine.HideInInspector]
	public PlatformAdvancedSettings AdvancedSettings;

	[UnityEngine.HideInInspector]
	public AkCommonCommSettings CommsSettings;
}
