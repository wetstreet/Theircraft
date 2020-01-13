#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkAcousticSurfaceArray : AkBaseArray<AkAcousticSurface>
{
	public AkAcousticSurfaceArray(int count) : base(count)
	{
	}

	protected override int StructureSize
	{
		get { return AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_GetSizeOf(); }
	}

	protected override void ClearAtIntPtr(System.IntPtr address)
	{
		AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_Clear(address);
	}

	protected override AkAcousticSurface CreateNewReferenceFromIntPtr(System.IntPtr address)
	{
		return new AkAcousticSurface(address, false);
	}

	protected override void CloneIntoReferenceFromIntPtr(System.IntPtr address, AkAcousticSurface other)
	{
		AkSoundEnginePINVOKE.CSharp_AkAcousticSurface_Clone(address, AkAcousticSurface.getCPtr(other));
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.