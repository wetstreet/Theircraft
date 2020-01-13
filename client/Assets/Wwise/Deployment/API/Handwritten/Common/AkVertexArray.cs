#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2012 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkVertexArray : AkBaseArray<AkVertex>
{
	public AkVertexArray(int count) : base(count)
	{
	}

	protected override int StructureSize
	{
		get { return AkSoundEnginePINVOKE.CSharp_AkVertex_GetSizeOf(); }
	}

	protected override void ClearAtIntPtr(System.IntPtr address)
	{
		AkSoundEnginePINVOKE.CSharp_AkVertex_Clear(address);
	}

	protected override AkVertex CreateNewReferenceFromIntPtr(System.IntPtr address)
	{
		return new AkVertex(address, false);
	}

	protected override void CloneIntoReferenceFromIntPtr(System.IntPtr address, AkVertex other)
	{
		AkSoundEnginePINVOKE.CSharp_AkVertex_Clone(address, AkVertex.getCPtr(other));
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.