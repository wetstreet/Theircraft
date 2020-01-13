//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise state groups as Unity assets.
public class WwiseStateGroupReference : WwiseObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.StateGroup;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseStateGroupReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
