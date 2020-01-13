//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise triggers as Unity assets.
public class WwiseTriggerReference : WwiseObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.Trigger;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseTriggerReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
