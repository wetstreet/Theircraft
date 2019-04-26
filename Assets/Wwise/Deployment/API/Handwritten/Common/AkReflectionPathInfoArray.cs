#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkReflectionPathInfoArray : AkBaseArray<AkReflectionPathInfo>
{
	public AkReflectionPathInfoArray(int count) : base(count)
	{
	}

	protected override int StructureSize
	{
		get { return AkSoundEnginePINVOKE.CSharp_AkReflectionPathInfo_GetSizeOf(); }
	}

	protected override AkReflectionPathInfo CreateNewReferenceFromIntPtr(System.IntPtr address)
	{
		return new AkReflectionPathInfo(address, false);
	}

	protected override void CloneIntoReferenceFromIntPtr(System.IntPtr address, AkReflectionPathInfo other)
	{
		AkSoundEnginePINVOKE.CSharp_AkReflectionPathInfo_Clone(address, AkReflectionPathInfo.getCPtr(other));
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.