#if UNITY_EDITOR

public class AkWaapiClient_PINVOKE {

	static AkWaapiClient_PINVOKE() {}


	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "SetWampEventCallback")]
	public static extern bool SetWampEventCallback(AkWaapiClient.WaapiEventCallback Callback);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint="Connect")]
	public static extern bool Connect(
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_uri,
		uint in_port
		);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "IsConnected")]
	public static extern bool IsConnected();

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "Disconnect")]
	public static extern void Disconnect();

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "Subscribe")]
	public static extern bool Subscribe(
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_uri,
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_options,
		out ulong SubscriptionID,
		out int resultLength
		);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "SubscribeWithTimeout")]
	public static extern bool Subscribe(
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_uri,
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_options,
		out ulong SubscriptionID,
		int TimeoutMs,
		out int resultLength
		);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "Unsubscribe")]
	public static extern bool Unsubscribe(
		ulong SubscriptionID,
		out int resultLength
		);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "UnsubscribeWithTimeout")]
	public static extern bool Unsubscribe(
		ulong SubscriptionID,
		int TimeoutMs,
        out int resultLength
		);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "Call")]
	public static extern bool Call(
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_uri,
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_args,
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_options,
        out int resultLength
		);

	[global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "CallWithTimeout")]
	public static extern bool Call(
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_uri,
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_args,
		[global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.LPStr)]string in_options,
		int TimeoutMs,
        out int resultLength
		);

    [global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "GetLastString")]
    public static extern bool GetLastString(
        System.Text.StringBuilder out_result,
        int resultLength
        );

    [global::System.Runtime.InteropServices.DllImport("AkWaapiClient", EntryPoint = "ProcessCallbacks")]
	public static extern void ProcessCallbacks();
}

#endif