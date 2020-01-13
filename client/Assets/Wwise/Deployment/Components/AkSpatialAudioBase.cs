#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2017 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

/// <summary>
///     Base class for spatial audio emitter and listener components.
/// </summary>
public abstract class AkSpatialAudioBase : UnityEngine.MonoBehaviour
{
	private readonly AkRoom.PriorityList roomPriorityList = new AkRoom.PriorityList();

	protected void SetGameObjectInHighestPriorityRoom()
	{
		var highestPriorityRoomID = roomPriorityList.GetHighestPriorityRoomID();
		AkSoundEngine.SetGameObjectInRoom(gameObject, highestPriorityRoomID);
	}

	/// <summary>
	///     Called when entering a room.
	/// </summary>
	/// <param name="room">The room.</param>
	public void EnteredRoom(AkRoom room)
	{
		roomPriorityList.Add(room);
		SetGameObjectInHighestPriorityRoom();
	}

	/// <summary>
	///     Called when exiting a room.
	/// </summary>
	/// <param name="room">The room.</param>
	public void ExitedRoom(AkRoom room)
	{
		roomPriorityList.Remove(room);
		SetGameObjectInHighestPriorityRoom();
	}

	/// <summary>
	///     Sets the Unity Game Object within the highest priority room.
	/// </summary>
	public void SetGameObjectInRoom()
	{
		var colliders = UnityEngine.Physics.OverlapSphere(transform.position, 0.0f);
		foreach (var collider in colliders)
		{
			var room = collider.gameObject.GetComponent<AkRoom>();
			if (room != null)
				roomPriorityList.Add(room);
		}

		SetGameObjectInHighestPriorityRoom();
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.