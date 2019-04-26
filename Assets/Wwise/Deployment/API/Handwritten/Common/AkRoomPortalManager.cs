#if !(UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2019 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// @brief Tracks when rooms are updated, and defers the update of portal room IDs.
public static class AkRoomPortalManager
{
	private static readonly System.Collections.Generic.List<AkRoom> m_UpdatedRooms = 
		new System.Collections.Generic.List<AkRoom>();

	private static readonly System.Collections.Generic.List<AkRoomPortal> m_Portals =
		new System.Collections.Generic.List<AkRoomPortal>();

	private static readonly System.Collections.Generic.List<AkRoomPortal> m_PortalsToUpdate =
		new System.Collections.Generic.List<AkRoomPortal>();

	public static void RegisterPortal(AkRoomPortal portal)
	{
		if (!m_Portals.Contains(portal))
		{
			m_Portals.Add(portal);
		}
	}

	public static void UnregisterPortal(AkRoomPortal portal)
	{
		if (m_Portals.Contains(portal))
		{
			m_Portals.Remove(portal);
		}
		if (m_PortalsToUpdate.Contains(portal))
		{
			m_PortalsToUpdate.Remove(portal);
		}
	}

	public static void RegisterRoomUpdate(AkRoom room)
	{
		m_UpdatedRooms.Add(room);
		foreach (AkRoomPortal portal in m_Portals)
		{
			if (!m_PortalsToUpdate.Contains(portal) && (room == portal.frontRoom || room == portal.backRoom))
			{
				m_PortalsToUpdate.Add(portal);
			}
		}
	}

	public static void UpdatePortals()
	{
		foreach (AkRoomPortal portal in m_PortalsToUpdate)
		{
			portal.UpdateSoundEngineRoomIDs();
		}
		m_UpdatedRooms.Clear();
		m_PortalsToUpdate.Clear();
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
