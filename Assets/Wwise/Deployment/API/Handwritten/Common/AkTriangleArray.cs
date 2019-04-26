#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkTriangleArray : AkBaseArray<AkTriangle>
{
	public AkTriangleArray(int count) : base(count)
	{
	}

	protected override int StructureSize
	{
		get { return AkSoundEnginePINVOKE.CSharp_AkTriangle_GetSizeOf(); }
	}

	protected override void ClearAtIntPtr(System.IntPtr address)
	{
		AkSoundEnginePINVOKE.CSharp_AkTriangle_Clear(address);
	}

	protected override AkTriangle CreateNewReferenceFromIntPtr(System.IntPtr address)
	{
		return new AkTriangle(address, false);
	}

	protected override void CloneIntoReferenceFromIntPtr(System.IntPtr address, AkTriangle other)
	{
		AkSoundEnginePINVOKE.CSharp_AkTriangle_Clone(address, AkTriangle.getCPtr(other));
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.