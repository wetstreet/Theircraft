public static class AkXboxOneUtils
{
	private static readonly int[] Usages = { 0, 1, 4, 7 };
	private static readonly int SessionRequirement = 0;
	private static readonly int[] DeviceUsages = { 0 };

	[UnityEditor.MenuItem("Assets/Wwise/Xbox One/Enable Network Sockets")]
	public static void EnableXboxOneNetworkSockets()
	{
		var definitions = new[]
		{
			new SocketDefinition("WwiseDiscoverySocket", "24024", 1, "WwiseDiscovery"),
			new SocketDefinition("WwiseCommandSocket", "24025", 0, "WwiseCommand"),
			new SocketDefinition("WwiseNotificationSocket", "24026", 0, "WwiseNotification")
		};

		foreach (var def in definitions)
		{
			UnityEditor.PlayerSettings.XboxOne.SetSocketDefinition(def.Name, def.Port, def.Protocol, Usages, def.TemplateName,
				SessionRequirement, DeviceUsages);
		}
	}

	private class SocketDefinition
	{
		public readonly string Name;
		public readonly string Port;
		public readonly int Protocol;
		public readonly string TemplateName;

		public SocketDefinition(string name, string port, int protocol, string templateName)
		{
			Name = name;
			Port = port;
			Protocol = protocol;
			TemplateName = templateName;
		}
	}
}