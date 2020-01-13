//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise states as Unity assets.
public class WwiseStateReference : WwiseGroupValueObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.State;
	private static readonly WwiseObjectType MyGroupWwiseObjectType = WwiseObjectType.StateGroup;

	[AkShowOnly]
	[UnityEngine.SerializeField]
	private WwiseStateGroupReference WwiseStateGroupReference;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

	public override WwiseObjectReference GroupObjectReference
	{
		get { return WwiseStateGroupReference; }
		set { WwiseStateGroupReference = value as WwiseStateGroupReference; }
	}

	public override WwiseObjectType GroupWwiseObjectType { get { return MyGroupWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseStateReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
