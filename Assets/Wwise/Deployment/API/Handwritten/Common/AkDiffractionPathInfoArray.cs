#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkDiffractionPathInfoArray : AkBaseArray<AkDiffractionPathInfo>
{
	public AkDiffractionPathInfoArray(int count) : base(count)
	{
	}

	protected override int StructureSize
	{
		get { return AkSoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_GetSizeOf(); }
	}

	protected override AkDiffractionPathInfo CreateNewReferenceFromIntPtr(System.IntPtr address)
	{
		return new AkDiffractionPathInfo(address, false);
	}

	protected override void CloneIntoReferenceFromIntPtr(System.IntPtr address, AkDiffractionPathInfo other)
	{
		AkSoundEnginePINVOKE.CSharp_AkDiffractionPathInfo_Clone(address, AkDiffractionPathInfo.getCPtr(other));
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.