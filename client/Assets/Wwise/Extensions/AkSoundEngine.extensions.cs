#if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public partial class AkSoundEngine
{
	#region User Hooks - Extended for Auto-Registration

	private class AutoObject
	{
		private readonly UnityEngine.GameObject gameObject;

		public AutoObject(UnityEngine.GameObject go)
		{
			gameObject = go;
			RegisterGameObj(gameObject, gameObject != null ? "AkAutoObject for " + gameObject.name : "AkAutoObject");
		}

		~AutoObject()
		{
			UnregisterGameObj(gameObject);
		}
	}

	private static void AutoRegister(UnityEngine.GameObject gameObject, ulong id)
	{
		if (gameObject == null || !gameObject.activeInHierarchy)
			new AutoObject(gameObject);
		else if (gameObject.GetComponent<AkGameObj>() == null)
			gameObject.AddComponent<AkGameObj>();
	}

	static partial void PreGameObjectAPICallUserHook(UnityEngine.GameObject gameObject, ulong id)
	{
#if UNITY_EDITOR
		if (!UnityEngine.Application.isPlaying)
		{
			AutoRegister(gameObject, id);
			return;
		}
#endif

		if (!IsInRegisteredList(id) && IsInitialized())
			AutoRegister(gameObject, id);
	}

	private static readonly System.Collections.Generic.HashSet<ulong> RegisteredGameObjects =
		new System.Collections.Generic.HashSet<ulong>();

	static partial void PostRegisterGameObjUserHook(AKRESULT result, UnityEngine.GameObject gameObject, ulong id)
	{
#if UNITY_EDITOR
		if (!UnityEngine.Application.isPlaying)
			return;
#endif

		if (result == AKRESULT.AK_Success)
			RegisteredGameObjects.Add(id);
	}

	static partial void PostUnregisterGameObjUserHook(AKRESULT result, UnityEngine.GameObject gameObject, ulong id)
	{
		if (result == AKRESULT.AK_Success)
			RegisteredGameObjects.Remove(id);
	}

	// Helper method that a user might want to implement
	private static bool IsInRegisteredList(ulong id)
	{
		return RegisteredGameObjects.Contains(id);
	}

	// Helper method that a user might want to implement
	public static bool IsGameObjectRegistered(UnityEngine.GameObject in_gameObject)
	{
#if UNITY_EDITOR
		if (!UnityEngine.Application.isPlaying)
			return in_gameObject.GetComponent<AkGameObj>() != null;
#endif

		return IsInRegisteredList(GetAkGameObjectID(in_gameObject));
	}

	#endregion
}
#endif // #if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.