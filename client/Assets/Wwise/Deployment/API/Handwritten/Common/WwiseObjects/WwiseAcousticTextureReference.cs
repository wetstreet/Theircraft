//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise banks as Unity assets.
public class WwiseAcousticTextureReference : WwiseObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.AcousticTexture;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseAcousticTextureReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
