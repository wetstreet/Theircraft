#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEngine.AddComponentMenu("Wwise/AkRoomPortalObstruction")]
[UnityEngine.RequireComponent(typeof(AkRoomPortal))]
/// @brief Obstructs/Occludes the spatial audio portal of the current game object from the spatial audio listener if at least one object is between them.
/// @details If no spatial audio listener has been registered, there will be no obstruction.
public class AkRoomPortalObstruction : AkObstructionOcclusion
{
	private AkRoomPortal m_portal;

	private void Awake()
	{
		InitIntervalsAndFadeRates();
		m_portal = GetComponent<AkRoomPortal>();
	}

	protected override void UpdateObstructionOcclusionValuesForListeners()
	{
		UpdateObstructionOcclusionValues(AkSpatialAudioListener.TheSpatialAudioListener);
	}

	protected override void SetObstructionOcclusion(
		System.Collections.Generic.KeyValuePair<AkAudioListener, ObstructionOcclusionValue> ObsOccPair)
	{
		if (m_portal.IsValid)
			AkSoundEngine.SetPortalObstructionAndOcclusion(m_portal.GetID(), ObsOccPair.Value.currentValue, 0.0f);
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.