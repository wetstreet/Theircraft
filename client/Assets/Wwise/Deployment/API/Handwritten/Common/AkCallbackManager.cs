#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// <summary>
///     This class manages the callback queue.  All callbacks from the native Wwise SDK go through this queue.
///     The queue needs to be driven by regular calls to PostCallbacks().  This is currently done in AkInitializer.cs, in
///     LateUpdate().
/// </summary>
public static class AkCallbackManager
{
	/// <summary>
	/// Event callback used when posting events.
	/// </summary>
	public delegate void EventCallback(object in_cookie, AkCallbackType in_type, AkCallbackInfo in_info);

	/// <summary>
	/// Monitoring callback called when Wwise reports errors.
	/// </summary>
	public delegate void MonitoringCallback(AkMonitorErrorCode in_errorCode, AkMonitorErrorLevel in_errorLevel,
		uint in_playingID, ulong in_gameObjID, string in_msg);

	/// <summary>
	/// Bank callback called upon bank load and unload and when errors occur.
	/// </summary>
	public delegate void BankCallback(uint in_bankID, System.IntPtr in_InMemoryBankPtr, AKRESULT in_eLoadResult,
		uint in_memPoolId, object in_Cookie);

	private static bool IsLoggingEnabled { get; set; }

	private static readonly AkEventCallbackInfo AkEventCallbackInfo = new AkEventCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkDynamicSequenceItemCallbackInfo AkDynamicSequenceItemCallbackInfo =
		new AkDynamicSequenceItemCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkMIDIEventCallbackInfo AkMIDIEventCallbackInfo =
		new AkMIDIEventCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkMarkerCallbackInfo
		AkMarkerCallbackInfo = new AkMarkerCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkDurationCallbackInfo AkDurationCallbackInfo =
		new AkDurationCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkMusicSyncCallbackInfo AkMusicSyncCallbackInfo =
		new AkMusicSyncCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkMusicPlaylistCallbackInfo AkMusicPlaylistCallbackInfo =
		new AkMusicPlaylistCallbackInfo(System.IntPtr.Zero, false);

#if UNITY_IOS && !UNITY_EDITOR
	private static AkAudioInterruptionCallbackInfo AkAudioInterruptionCallbackInfo =
		new AkAudioInterruptionCallbackInfo(System.IntPtr.Zero, false);
#endif // #if UNITY_IOS && ! UNITY_EDITOR

	private static readonly AkAudioSourceChangeCallbackInfo AkAudioSourceChangeCallbackInfo =
		new AkAudioSourceChangeCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkMonitoringCallbackInfo AkMonitoringCallbackInfo =
		new AkMonitoringCallbackInfo(System.IntPtr.Zero, false);

	private static readonly AkBankCallbackInfo AkBankCallbackInfo = new AkBankCallbackInfo(System.IntPtr.Zero, false);

	/// <summary>
	/// This class holds the data associated with an event callback.
	/// </summary>
	public class EventCallbackPackage
	{
		public bool m_bNotifyEndOfEvent;
		public EventCallback m_Callback;

		public object m_Cookie;
		public uint m_playingID;

		public static EventCallbackPackage Create(EventCallback in_cb, object in_cookie, ref uint io_Flags)
		{
			if (io_Flags == 0 || in_cb == null)
			{
				io_Flags = 0;
				return null;
			}

			var evt = new EventCallbackPackage();

			evt.m_Callback = in_cb;
			evt.m_Cookie = in_cookie;
			evt.m_bNotifyEndOfEvent = (io_Flags & (uint) AkCallbackType.AK_EndOfEvent) != 0;
			io_Flags = io_Flags | (uint) AkCallbackType.AK_EndOfEvent;

			m_mapEventCallbacks[evt.GetHashCode()] = evt;
			m_LastAddedEventPackage = evt;

			return evt;
		}

		~EventCallbackPackage()
		{
			if (m_Cookie != null)
				RemoveEventCallbackCookie(m_Cookie);
		}
	}

	/// <summary>
	/// This class holds the data associated with a bank load or unload callback.
	/// </summary>
	public class BankCallbackPackage
	{
		public BankCallback m_Callback;
		public object m_Cookie;

		public BankCallbackPackage(BankCallback in_cb, object in_cookie)
		{
			m_Callback = in_cb;
			m_Cookie = in_cookie;

			m_mapBankCallbacks[GetHashCode()] = this;
		}
	}

	private static readonly System.Collections.Generic.Dictionary<int, EventCallbackPackage> m_mapEventCallbacks =
		new System.Collections.Generic.Dictionary<int, EventCallbackPackage>();

	private static readonly System.Collections.Generic.Dictionary<int, BankCallbackPackage> m_mapBankCallbacks =
		new System.Collections.Generic.Dictionary<int, BankCallbackPackage>();

	private static EventCallbackPackage m_LastAddedEventPackage;

	public static void RemoveEventCallback(uint in_playingID)
	{
		var cookiesToRemove = new System.Collections.Generic.List<int>();
		foreach (var pair in m_mapEventCallbacks)
		{
			if (pair.Value.m_playingID == in_playingID)
			{
				cookiesToRemove.Add(pair.Key);
				break;
			}
		}

		var Count = cookiesToRemove.Count;
		for (var ii = 0; ii < Count; ++ii)
			m_mapEventCallbacks.Remove(cookiesToRemove[ii]);

		AkSoundEnginePINVOKE.CSharp_CancelEventCallback(in_playingID);
	}

	public static void RemoveEventCallbackCookie(object in_cookie)
	{
		var cookiesToRemove = new System.Collections.Generic.List<int>();
		foreach (var pair in m_mapEventCallbacks)
		{
			if (pair.Value.m_Cookie == in_cookie)
				cookiesToRemove.Add(pair.Key);
		}

		var Count = cookiesToRemove.Count;
		for (var ii = 0; ii < Count; ++ii)
		{
			var toRemove = cookiesToRemove[ii];
			m_mapEventCallbacks.Remove(toRemove);
			AkSoundEnginePINVOKE.CSharp_CancelEventCallbackCookie((System.IntPtr) toRemove);
		}
	}

	public static void RemoveBankCallback(object in_cookie)
	{
		var cookiesToRemove = new System.Collections.Generic.List<int>();
		foreach (var pair in m_mapBankCallbacks)
		{
			if (pair.Value.m_Cookie == in_cookie)
				cookiesToRemove.Add(pair.Key);
		}

		var Count = cookiesToRemove.Count;
		for (var ii = 0; ii < Count; ++ii)
		{
			var toRemove = cookiesToRemove[ii];
			m_mapBankCallbacks.Remove(toRemove);
			AkSoundEnginePINVOKE.CSharp_CancelBankCallbackCookie((System.IntPtr) toRemove);
		}
	}

	public static void SetLastAddedPlayingID(uint in_playingID)
	{
		if (m_LastAddedEventPackage != null && m_LastAddedEventPackage.m_playingID == 0)
			m_LastAddedEventPackage.m_playingID = in_playingID;
	}

	private static System.IntPtr m_pNotifMem = System.IntPtr.Zero;
	private static MonitoringCallback m_MonitoringCB;

#if UNITY_IOS && !UNITY_EDITOR
	public delegate AKRESULT AudioInterruptionCallback(bool in_bEnterInterruption, object in_Cookie);
	// App implements its own callback.
	private static AudioInterruptionCallbackPackage ms_interruptCallbackPkg = null;

	public class AudioInterruptionCallbackPackage
	{
		public object m_Cookie;
		public AudioInterruptionCallback m_Callback;
	}
#endif // #if UNITY_IOS && ! UNITY_EDITOR

	public delegate AKRESULT BGMCallback(bool in_bOtherAudioPlaying, object in_Cookie);

	// App implements its own callback.
	private static BGMCallbackPackage ms_sourceChangeCallbackPkg;

	public class BGMCallbackPackage
	{
		public BGMCallback m_Callback;
		public object m_Cookie;
	}

	public class InitializationSettings
	{
		public static int DefaultBufferSize = 4 * 1024;
		public static bool DefaultIsLoggingEnabled = true;

		public int BufferSize = DefaultBufferSize;
		public bool IsLoggingEnabled = DefaultIsLoggingEnabled;
	}

	public static AKRESULT Init(InitializationSettings settings)
	{
		IsLoggingEnabled = settings.IsLoggingEnabled;

		m_pNotifMem = settings.BufferSize > 0 ? System.Runtime.InteropServices.Marshal.AllocHGlobal(settings.BufferSize) : System.IntPtr.Zero;

#if UNITY_EDITOR
		AkCallbackSerializer.SetLocalOutput((uint) AkMonitorErrorLevel.ErrorLevel_All);
#endif

		return AkCallbackSerializer.Init(m_pNotifMem, (uint)settings.BufferSize);
	}

	public static void Term()
	{
		if (m_pNotifMem != System.IntPtr.Zero)
		{
			AkCallbackSerializer.Term();
			System.Runtime.InteropServices.Marshal.FreeHGlobal(m_pNotifMem);
			m_pNotifMem = System.IntPtr.Zero;
		}
	}

	/// Call this to set a function to call whenever Wwise prints a message (warnings or errors).
	public static void SetMonitoringCallback(AkMonitorErrorLevel in_Level, MonitoringCallback in_CB)
	{
		AkCallbackSerializer.SetLocalOutput(in_CB != null ? (uint) in_Level : 0);
		m_MonitoringCB = in_CB;
	}

#if UNITY_IOS && !UNITY_EDITOR
	/// Call this function to set a iOS callback interruption function. By default this callback is not defined.
	public static void SetInterruptionCallback(AudioInterruptionCallback in_CB, object in_cookie)
	{
		ms_interruptCallbackPkg = new AudioInterruptionCallbackPackage { m_Callback = in_CB, m_Cookie = in_cookie };
	}
#endif // #if UNITY_IOS && ! UNITY_EDITOR

	/// Call this to set a background music callback function. By default this callback is not defined.
	public static void SetBGMCallback(BGMCallback in_CB, object in_cookie)
	{
		ms_sourceChangeCallbackPkg = new BGMCallbackPackage { m_Callback = in_CB, m_Cookie = in_cookie };
	}

	/// This function dispatches all the accumulated callbacks from the native sound engine. 
	/// It must be called regularly.  By default this is called in AkInitializer.cs.
	public static int PostCallbacks()
	{
		if (m_pNotifMem == System.IntPtr.Zero)
			return 0;

		try
		{
			var numCallbacks = 0;

			for (var pNext = AkCallbackSerializer.Lock();
				pNext != System.IntPtr.Zero;
				pNext = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_pNext_get(pNext), ++numCallbacks)
			{
				var pPackage = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_pPackage_get(pNext);
				var eType = (AkCallbackType) AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_eType_get(pNext);
				var pData = AkSoundEnginePINVOKE.CSharp_AkSerializedCallbackHeader_GetData(pNext);

				switch (eType)
				{
					case AkCallbackType.AK_AudioInterruption:
#if UNITY_IOS && !UNITY_EDITOR
						if (ms_interruptCallbackPkg != null && ms_interruptCallbackPkg.m_Callback != null)
						{
							AkAudioInterruptionCallbackInfo.setCPtr(pData);
							ms_interruptCallbackPkg.m_Callback(AkAudioInterruptionCallbackInfo.bEnterInterruption, ms_interruptCallbackPkg.m_Cookie);
						}
#endif // #if UNITY_IOS && ! UNITY_EDITOR
						break;

					case AkCallbackType.AK_AudioSourceChange:
						if (ms_sourceChangeCallbackPkg != null && ms_sourceChangeCallbackPkg.m_Callback != null)
						{
							AkAudioSourceChangeCallbackInfo.setCPtr(pData);
							ms_sourceChangeCallbackPkg.m_Callback(AkAudioSourceChangeCallbackInfo.bOtherAudioPlaying,
								ms_sourceChangeCallbackPkg.m_Cookie);
						}
						break;

					case AkCallbackType.AK_Monitoring:
						if (m_MonitoringCB != null)
						{
							AkMonitoringCallbackInfo.setCPtr(pData);
							m_MonitoringCB(AkMonitoringCallbackInfo.errorCode, AkMonitoringCallbackInfo.errorLevel,
								AkMonitoringCallbackInfo.playingID, AkMonitoringCallbackInfo.gameObjID, AkMonitoringCallbackInfo.message);
						}
#if UNITY_EDITOR
						else if (IsLoggingEnabled)
						{
							AkMonitoringCallbackInfo.setCPtr(pData);

							var msg = "Wwise: " + AkMonitoringCallbackInfo.message;
							if (AkMonitoringCallbackInfo.gameObjID != AkSoundEngine.AK_INVALID_GAME_OBJECT)
							{
								var obj =
									UnityEditor.EditorUtility.InstanceIDToObject((int) AkMonitoringCallbackInfo.gameObjID) as
										UnityEngine.GameObject;
								if (obj != null)
									msg += " (GameObject: " + obj + ")";

								msg += " (Instance ID: " + AkMonitoringCallbackInfo.gameObjID + ")";
							}

							if (AkMonitoringCallbackInfo.errorLevel == AkMonitorErrorLevel.ErrorLevel_Error)
								UnityEngine.Debug.LogError(msg);
							else
								UnityEngine.Debug.Log(msg);
						}
#endif
						break;

					case AkCallbackType.AK_Bank:
						BankCallbackPackage bankPkg = null;
						if (!m_mapBankCallbacks.TryGetValue((int) pPackage, out bankPkg))
						{
							UnityEngine.Debug.LogError("WwiseUnity: BankCallbackPackage not found for <" + pPackage + ">.");
							return numCallbacks;
						}
						else
						{
							m_mapBankCallbacks.Remove((int) pPackage);

							if (bankPkg != null && bankPkg.m_Callback != null)
							{
								AkBankCallbackInfo.setCPtr(pData);
								bankPkg.m_Callback(AkBankCallbackInfo.bankID, AkBankCallbackInfo.inMemoryBankPtr, AkBankCallbackInfo.loadResult,
									(uint) AkBankCallbackInfo.memPoolId, bankPkg.m_Cookie);
							}
						}
						break;

					default:
						EventCallbackPackage eventPkg = null;
						if (!m_mapEventCallbacks.TryGetValue((int) pPackage, out eventPkg))
						{
							UnityEngine.Debug.LogError("WwiseUnity: EventCallbackPackage not found for <" + pPackage + ">.");
							return numCallbacks;
						}
						else
						{
							AkCallbackInfo info = null;

							switch (eType)
							{
								case AkCallbackType.AK_EndOfEvent:
									m_mapEventCallbacks.Remove(eventPkg.GetHashCode());
									if (eventPkg.m_bNotifyEndOfEvent)
									{
										AkEventCallbackInfo.setCPtr(pData);
										info = AkEventCallbackInfo;
									}
									break;

								case AkCallbackType.AK_MusicPlayStarted:
									AkEventCallbackInfo.setCPtr(pData);
									info = AkEventCallbackInfo;
									break;

								case AkCallbackType.AK_EndOfDynamicSequenceItem:
									AkDynamicSequenceItemCallbackInfo.setCPtr(pData);
									info = AkDynamicSequenceItemCallbackInfo;
									break;

								case AkCallbackType.AK_MIDIEvent:
									AkMIDIEventCallbackInfo.setCPtr(pData);
									info = AkMIDIEventCallbackInfo;
									break;

								case AkCallbackType.AK_Marker:
									AkMarkerCallbackInfo.setCPtr(pData);
									info = AkMarkerCallbackInfo;
									break;

								case AkCallbackType.AK_Duration:
									AkDurationCallbackInfo.setCPtr(pData);
									info = AkDurationCallbackInfo;
									break;

								case AkCallbackType.AK_MusicSyncUserCue:
								case AkCallbackType.AK_MusicSyncBar:
								case AkCallbackType.AK_MusicSyncBeat:
								case AkCallbackType.AK_MusicSyncEntry:
								case AkCallbackType.AK_MusicSyncExit:
								case AkCallbackType.AK_MusicSyncGrid:
								case AkCallbackType.AK_MusicSyncPoint:
									AkMusicSyncCallbackInfo.setCPtr(pData);
									info = AkMusicSyncCallbackInfo;
									break;

								case AkCallbackType.AK_MusicPlaylistSelect:
									AkMusicPlaylistCallbackInfo.setCPtr(pData);
									info = AkMusicPlaylistCallbackInfo;
									break;

								default:
									UnityEngine.Debug.LogError("WwiseUnity: PostCallbacks aborted due to error: Undefined callback type <" +
									                           eType + "> found. Callback object possibly corrupted.");
									return numCallbacks;
							}

							if (info != null)
								eventPkg.m_Callback(eventPkg.m_Cookie, eType, info);
						}
						break;
				}
			}

			return numCallbacks;
		}
		finally
		{
			AkCallbackSerializer.Unlock();
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.