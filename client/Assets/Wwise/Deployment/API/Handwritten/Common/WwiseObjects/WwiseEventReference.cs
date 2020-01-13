//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise events as Unity assets.
public class WwiseEventReference : WwiseObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.Event;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseEventReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
