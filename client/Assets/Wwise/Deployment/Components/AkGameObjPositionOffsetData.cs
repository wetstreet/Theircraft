#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[System.Serializable]
public class AkGameObjPositionOffsetData
{
	public bool KeepMe;
	public UnityEngine.Vector3 positionOffset;

	// Unity tries to construct a AkGameObjPositionOffsetData all the time. Need this ugly workaround
	// to prevent it from doing this.
	public AkGameObjPositionOffsetData(bool IReallyWantToBeConstructed = false)
	{
		KeepMe = IReallyWantToBeConstructed;
	}
}

#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.