#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkEventCallbackData : UnityEngine.ScriptableObject
{
	////AkSoundEngine.PostEvent callback flags. See the AkCallbackType enumeration for a list of all callbacks
	public System.Collections.Generic.List<int> callbackFlags = new System.Collections.Generic.List<int>();

	////Names of the callback functions.
	public System.Collections.Generic.List<string> callbackFunc = new System.Collections.Generic.List<string>();

	////GameObject that will receive the callback
	public System.Collections.Generic.List<UnityEngine.GameObject> callbackGameObj =
		new System.Collections.Generic.List<UnityEngine.GameObject>();

	////The sum of the flags of all game objects. This is the flag that will be passed to AkSoundEngine.PostEvent
	public int uFlags = 0;
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.