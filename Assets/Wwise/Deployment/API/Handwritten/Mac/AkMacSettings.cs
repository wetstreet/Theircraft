public class AkMacSettings : AkWwiseInitializationSettings.CommonPlatformSettings
{
#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticPlatformRegistration()
	{
		RegisterPlatformSettingsClass<AkMacSettings>("Mac");
	}
#endif // UNITY_EDITOR
}
