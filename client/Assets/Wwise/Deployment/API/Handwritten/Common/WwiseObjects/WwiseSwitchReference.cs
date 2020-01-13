//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2018 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Represents Wwise states as Unity assets.
public class WwiseSwitchReference : WwiseGroupValueObjectReference
{
	private static readonly WwiseObjectType MyWwiseObjectType = WwiseObjectType.Switch;
	private static readonly WwiseObjectType MyGroupWwiseObjectType = WwiseObjectType.SwitchGroup;

	[AkShowOnly]
	[UnityEngine.SerializeField]
	private WwiseSwitchGroupReference WwiseSwitchGroupReference;

	public override WwiseObjectType WwiseObjectType { get { return MyWwiseObjectType; } }

	public override WwiseObjectReference GroupObjectReference
	{
		get { return WwiseSwitchGroupReference; }
		set { WwiseSwitchGroupReference = value as WwiseSwitchGroupReference; }
	}

	public override WwiseObjectType GroupWwiseObjectType { get { return MyGroupWwiseObjectType; } }

#if UNITY_EDITOR
	[UnityEditor.InitializeOnLoadMethod]
	private static void AutomaticClassRegistration()
	{
		RegisterWwiseObjectReferenceClass<WwiseSwitchReference>(MyWwiseObjectType);
	}
#endif // UNITY_EDITOR
}
