#if UNITY_EDITOR

[UnityEditor.InitializeOnLoad]
///@brief The full Wwise Authoring API is exposed to Unity as a native plugin for the Windows and macOS platforms. All features, including subscriptions, are supported through the \c AkWAAPIClient class.
public class AkWaapiClient
{
	#region WAAPI Client Interface
	/// <summary>
	///     Connects to a Wwise Authoring application
	/// </summary>
	/// <param name="Uri">IP to connect to</param>
	/// <param name="Port">Port to connect to</param>
	/// <returns>Connection success</returns>
	public static bool Connect(string Uri, uint Port)
	{
		if (!IsConnected)
		{
			return AkWaapiClient_PINVOKE.Connect(Uri, Port);
		}

		return true;
	}

	/// <summary>
	///     Is the client connected
	/// </summary>
	public static bool IsConnected
	{
		get { return AkWaapiClient_PINVOKE.IsConnected(); }
	}

	/// <summary>
	///     Disconnects from the connected Wwise Authoring application
	/// </summary>
	public static void Disconnect()
	{
		foreach (var Callback in WaapiEventCallbacks)
		{
			string Result;
			UnsubscribeInternal(Callback.Key, -1, out Result);
		}

		WaapiEventCallbacks.Clear();
		AkWaapiClient_PINVOKE.Disconnect();
	}

	/// <summary>
	///     Subscribes to a topic. See <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=waapi__subscribe.html" target="_blank">Subscribing to Topics in the Wwise Authoring API</a> for more information.
	/// </summary>
	/// <param name="Uri">Topic to subscribe to</param>
	/// <param name="Options">Elements of the Wwise objects to return</param>
	/// <param name="Callback">C# delegate to execute when subscription is triggered</param>
	/// <param name="SubscriptionID">Unique ID representing the subscription</param>
	/// <param name="Result">Result of the subscription attempt</param>
	/// <returns>Subscription success</returns>
	public static bool Subscribe(string Uri, string Options, WaapiEventCallback Callback, out ulong SubscriptionID, out string Result)
	{
		int ResultLength;
		bool res = AkWaapiClient_PINVOKE.Subscribe(Uri, Options, out SubscriptionID, out ResultLength);
		if (res)
		{
			WaapiEventCallbacks.Add(SubscriptionID, Callback);
		}
		res &= GetLastString(ResultLength, out Result);
		return res;
	}

	/// <summary>
	///     Subscribes to a topic. See <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=waapi__subscribe.html" target="_blank">Subscribing to Topics in the Wwise Authoring API</a> for more information.
	/// </summary>
	/// <param name="Uri">Topic to subscribe to</param>
	/// <param name="Options">Elements of the Wwise objects to return</param>
	/// <param name="Callback">C# delegate to execute when subscription is triggered</param>
	/// <param name="TimeoutMs">Timeout in milliseconds to wait on the Wwise Authoring application to respond</param>
	/// <param name="SubscriptionID">Unique ID representing the subscription</param>
	/// <param name="Result">Result of the subscription attempt</param>
	/// <returns>Subscription success</returns>
	public static bool Subscribe(string Uri, string Options, WaapiEventCallback Callback, int TimeoutMs, out ulong SubscriptionID, out string Result)
	{
		int ResultLength;
		bool res = AkWaapiClient_PINVOKE.Subscribe(Uri, Options, out SubscriptionID, TimeoutMs, out ResultLength);
		if (res)
		{
			WaapiEventCallbacks.Add(SubscriptionID, Callback);
		}
		res &= GetLastString(ResultLength, out Result);
		return res;
	}

	private static bool UnsubscribeInternal(ulong SubscriptionID, int TimeoutMs, out string Result)
	{
		int ResultLength;
		bool res = AkWaapiClient_PINVOKE.Unsubscribe(SubscriptionID, TimeoutMs, out ResultLength);
		res &= GetLastString(ResultLength, out Result);
		return res;
	}

	/// <summary>
	///     Unsubscribes from a topic.
	/// </summary>
	/// <param name="SubscriptionID">Unique ID representing the subscription</param>
	/// <param name="Result">Result of the unsubscription attempt</param>
	/// <returns>Unsubscription success</returns>
	public static bool Unsubscribe(ulong SubscriptionID, out string Result)
	{
		bool res = UnsubscribeInternal(SubscriptionID, -1, out Result);
		if (res)
		{
			WaapiEventCallbacks.Remove(SubscriptionID);
		}
		return res;
	}

	/// <summary>
	///     Unsubscribes from a topic.
	/// </summary>
	/// <param name="SubscriptionID">Unique ID representing the subscription</param>
	/// <param name="TimeoutMs">Timeout in milliseconds to wait on the Wwise Authoring application to respond</param>
	/// <param name="Result">Result of the unsubscription attempt</param>
	/// <returns>Unsubscription success</returns>
	public static bool Unsubscribe(ulong SubscriptionID, int TimeoutMs, out string Result)
	{
		bool res = UnsubscribeInternal(SubscriptionID, TimeoutMs, out Result);
		if (res)
		{
			WaapiEventCallbacks.Remove(SubscriptionID);
		}
		return res;
	}

	/// <summary>
	///     Perform a call to the Wwise Authoring. See <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=waapi__query.html" target="_blank">Querying the Wwise Project</a> for more information.
	/// </summary>
	/// <param name="Uri">Function to call</param>
	/// <param name="Args">Arguments required by the function</param>
	/// <param name="Options">Elements of the Wwise objects to return</param>
	/// <param name="Result">Result of the subscription attempt</param>
	/// <returns>Call success</returns>
	public static bool Call(string Uri, string Args, string Options, out string Result)
	{
		int ResultLength;
		bool res = AkWaapiClient_PINVOKE.Call(Uri, Args, Options, out ResultLength);
		res &= GetLastString(ResultLength, out Result);
		return res;
	}

	/// <summary>
	///     Perform a call to the Wwise Authoring. See <a href="https://www.audiokinetic.com/library/edge/?source=SDK&id=waapi__query.html" target="_blank">Querying the Wwise Project</a> for more information.
	/// </summary>
	/// <param name="Uri">Function to call</param>
	/// <param name="Args">Arguments required by the function</param>
	/// <param name="Options">Elements of the Wwise objects to return</param>
	/// <param name="TimeoutMs">Timeout in milliseconds to wait on the Wwise Authoring application to respond</param>
	/// <param name="Result">Result of the subscription attempt</param>
	/// <returns>Call success</returns>
	public static bool Call(string Uri, string Args, string Options, int TimeoutMs, out string Result)
	{
		int ResultLength;
		bool res = AkWaapiClient_PINVOKE.Call(Uri, Args, Options, TimeoutMs, out ResultLength);
		res &= GetLastString(ResultLength, out Result);
		return res;
	}

	#endregion

	#region Callback Management

	/// <summary>
	///     Delegate ran when a subscription is triggered
	/// </summary>
	/// <param name="SubscriptionID">Unique ID representing the subscription</param>
	/// <param name="Contents">Information requested in the subscription</param>
	[System.Runtime.InteropServices.UnmanagedFunctionPointer(System.Runtime.InteropServices.CallingConvention.StdCall)]
	public delegate void WaapiEventCallback(ulong SubscriptionID, string Contents);

	private static void ProcessCallbacks()
	{
		if (UnityEditor.EditorApplication.isCompiling)
		{
			UnityEditor.EditorApplication.update -= ProcessCallbacks;
		}
		else if (IsConnected)
		{
			AkWaapiClient_PINVOKE.ProcessCallbacks();
		}
	}

	[AOT.MonoPInvokeCallback(typeof(WaapiEventCallback))]
	private static void InternalWampEventCallback(ulong SubscriptionID, string Contents)
	{
		if (WaapiEventCallbacks.ContainsKey(SubscriptionID))
		{
			WaapiEventCallbacks[SubscriptionID](SubscriptionID, Contents);
		}
		else
		{
			string Result;
			UnityEngine.Debug.Log("SubscriptionID " + SubscriptionID + " not found");
			UnsubscribeInternal(SubscriptionID, -1, out Result);
		}
	}

	private static System.Collections.Generic.Dictionary<ulong, WaapiEventCallback> WaapiEventCallbacks
		= new System.Collections.Generic.Dictionary<ulong, WaapiEventCallback>();
	#endregion

	static AkWaapiClient()
	{
		UnityEditor.EditorApplication.update += ProcessCallbacks;
		AkWaapiClient_PINVOKE.SetWampEventCallback(InternalWampEventCallback);
	}

	~AkWaapiClient()
	{
		UnityEditor.EditorApplication.update -= ProcessCallbacks;
	}

	private static bool GetLastString(int StringLength, out string Result)
	{
		System.Text.StringBuilder ResultBuilder = new System.Text.StringBuilder(StringLength);
		bool res = AkWaapiClient_PINVOKE.GetLastString(ResultBuilder, ResultBuilder.Capacity);
		Result = ResultBuilder.ToString();
		return res;
	}
}
#endif
