#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
[UnityEngine.AddComponentMenu("Wwise/AkRoom")]
[UnityEngine.RequireComponent(typeof(UnityEngine.Collider))]
[UnityEngine.DisallowMultipleComponent]
/// @brief An AkRoom is an enclosed environment that can only communicate to the outside/other rooms with AkRoomPortals
/// @details 
public class AkRoom : AkTriggerHandler
{
	public static ulong INVALID_ROOM_ID = unchecked((ulong)-1.0f);

	private static int RoomCount;

	[UnityEngine.Tooltip("Higher number has a higher priority")]
	/// In cases where a game object is in an area with two rooms, the higher priority room will be chosen for AK::SpatialAudio::SetGameObjectInRoom()
	/// The higher the priority number, the higher the priority of a room.
	public int priority = 0;

	/// The reverb auxiliary bus.
	public AK.Wwise.AuxBus reverbAuxBus = new AK.Wwise.AuxBus();

	[UnityEngine.Range(0, 1)]
	/// The reverb control value for the send to the reverb aux bus.
	public float reverbLevel = 1;

	[UnityEngine.Range(0, 1)]
	/// Occlusion level modeling transmission through walls.
	public float wallOcclusion = 1;

	/// Wwise Event to be posted on the room game object.
	public AK.Wwise.Event roomToneEvent = new AK.Wwise.Event();

	[UnityEngine.Range(0, 1)]
	[UnityEngine.Tooltip("Send level for sounds that are posted on the room game object; adds reverb to ambience and room tones. Valid range: (0.f-1.f). A value of 0 disables the aux send.")]
	/// Send level for sounds that are posted on the room game object; adds reverb to ambience and room tones. Valid range: (0.f-1.f). A value of 0 disables the aux send.
	public float roomToneAuxSend = 0;

	public static bool IsSpatialAudioEnabled
	{
		get { return AkSpatialAudioListener.TheSpatialAudioListener != null && RoomCount > 0; }
	}

	/// Access the room's ID
	public ulong GetID()
	{
		return AkSoundEngine.GetAkGameObjectID(gameObject);
	}

	private void OnEnable()
	{
		var roomParams = new AkRoomParams();

		roomParams.Up.X = transform.up.x;
		roomParams.Up.Y = transform.up.y;
		roomParams.Up.Z = transform.up.z;

		roomParams.Front.X = transform.forward.x;
		roomParams.Front.Y = transform.forward.y;
		roomParams.Front.Z = transform.forward.z;

		roomParams.ReverbAuxBus = reverbAuxBus.Id;
		roomParams.ReverbLevel = reverbLevel;
		roomParams.WallOcclusion = wallOcclusion;

		roomParams.RoomGameObj_AuxSendLevelToSelf = roomToneAuxSend;
		roomParams.RoomGameObj_KeepRegistered = roomToneEvent.IsValid() ? true : false;

		RoomCount++;
		AkSoundEngine.SetRoom(GetID(), roomParams, name);

		/// In case a room is disbled and re-enabled. 
		AkRoomPortalManager.RegisterRoomUpdate(this);
	}

	public override void HandleEvent(UnityEngine.GameObject in_gameObject)
	{
		roomToneEvent.Post(gameObject);
	}

	private void OnDisable()
	{
		AkRoomPortalManager.RegisterRoomUpdate(this);

		RoomCount--;
		AkSoundEngine.RemoveRoom(GetID());
	}

	private void OnTriggerEnter(UnityEngine.Collider in_other)
	{
		var spatialAudioObjects = in_other.GetComponentsInChildren<AkSpatialAudioBase>();
		for (var i = 0; i < spatialAudioObjects.Length; i++)
		{
			if (spatialAudioObjects[i].enabled)
				spatialAudioObjects[i].EnteredRoom(this);
		}
	}

	private void OnTriggerExit(UnityEngine.Collider in_other)
	{
		var spatialAudioObjects = in_other.GetComponentsInChildren<AkSpatialAudioBase>();
		for (var i = 0; i < spatialAudioObjects.Length; i++)
		{
			if (spatialAudioObjects[i].enabled)
				spatialAudioObjects[i].ExitedRoom(this);
		}
	}

	public class PriorityList
	{
		private static readonly CompareByPriority s_compareByPriority = new CompareByPriority();

		/// Contains all active rooms sorted by priority.
		public System.Collections.Generic.List<AkRoom> rooms = new System.Collections.Generic.List<AkRoom>();

		public ulong GetHighestPriorityRoomID()
		{
			var room = GetHighestPriorityRoom();
			return room == null ? INVALID_ROOM_ID : room.GetID();
		}

		public AkRoom GetHighestPriorityRoom()
		{
			if (rooms.Count == 0)
				return null;

			return rooms[0];
		}

		public void Add(AkRoom room)
		{
			var index = BinarySearch(room);
			if (index < 0)
				rooms.Insert(~index, room);
		}

		public void Remove(AkRoom room)
		{
			rooms.Remove(room);
		}

		public bool Contains(AkRoom room)
		{
			return BinarySearch(room) >= 0;
		}

		public int BinarySearch(AkRoom room)
		{
			if (room)
				return rooms.BinarySearch(room, s_compareByPriority);
			else
				return -1;
		}

		private class CompareByPriority : System.Collections.Generic.IComparer<AkRoom>
		{
			public virtual int Compare(AkRoom a, AkRoom b)
			{
				var result = a.priority.CompareTo(b.priority);

				if (result == 0 && a != b)
					return 1;

				return -result; // inverted to have highest priority first
			}
		}
	}
}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.