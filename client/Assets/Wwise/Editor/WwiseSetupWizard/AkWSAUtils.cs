public static class AkWSAUtils
{
	[UnityEditor.MenuItem("Assets/Wwise/WSA/Enable PrivateNetworkClientServer Capability")]
	public static void EnablePrivateNetworkClientServer()
	{
		UnityEditor.PlayerSettings.WSA.SetCapability(UnityEditor.PlayerSettings.WSACapability.PrivateNetworkClientServer, true);
	}
}