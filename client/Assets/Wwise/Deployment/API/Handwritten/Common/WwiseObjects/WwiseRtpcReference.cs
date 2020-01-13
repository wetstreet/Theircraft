//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise RTPCs as Unity assets.
public class WwiseRtpcReference : WwiseObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.GameParameter;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseRtpcReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
