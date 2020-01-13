#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AkRoomPortal))]
public class AkRoomPortalInspector : UnityEditor.Editor
{
	private readonly AkUnityEventHandlerInspector m_ClosePortalEventHandlerInspector = new AkUnityEventHandlerInspector();
	private readonly AkUnityEventHandlerInspector m_OpenPortalEventHandlerInspector = new AkUnityEventHandlerInspector();

	private readonly int[] m_selectedIndex = new int[2];
	private readonly AkRoom.PriorityList[] roomList = { new AkRoom.PriorityList(), new AkRoom.PriorityList() };

	private AkRoomPortal m_roomPortal;

	[UnityEditor.MenuItem("GameObject/Wwise/Room Portal", false, 1)]
	public static void CreatePortal()
	{
		var portal = new UnityEngine.GameObject("RoomPortal");

		UnityEditor.Undo.AddComponent<AkRoomPortal>(portal);
		portal.GetComponent<UnityEngine.Collider>().isTrigger = true;

		UnityEditor.Selection.objects = new UnityEngine.Object[] { portal };
	}

	private void OnEnable()
	{
		m_OpenPortalEventHandlerInspector.Init(serializedObject, "triggerList", "Open On: ", false);
		m_ClosePortalEventHandlerInspector.Init(serializedObject, "closePortalTriggerList", "Close On: ", false);

		m_roomPortal = target as AkRoomPortal;

		m_roomPortal.FindOverlappingRooms(roomList);
		for (var i = 0; i < 2; i++)
		{
			var index = roomList[i].BinarySearch(m_roomPortal.GetRoom(i));
			m_selectedIndex[i] = index == -1 ? 0 : index;
		}
	}

	public override void OnInspectorGUI()
	{
		serializedObject.Update();

		m_OpenPortalEventHandlerInspector.OnGUI();
		m_ClosePortalEventHandlerInspector.OnGUI();

		m_roomPortal.FindOverlappingRooms(roomList);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			var labels = new string[2] { "Back", "Front" };

			for (var i = 0; i < 2; i++)
			{
				var roomListCount = roomList[i].rooms.Count;
				var roomLabels = new string[roomListCount];

				for (var j = 0; j < roomListCount; j++)
					roomLabels[j] = j + 1 + ". " + roomList[i].rooms[j].name;

				m_selectedIndex[i] = UnityEditor.EditorGUILayout.Popup(labels[i] + " Room",
					UnityEngine.Mathf.Clamp(m_selectedIndex[i], 0, roomListCount - 1), roomLabels);

				m_roomPortal.SetRoom(i, 
					m_selectedIndex[i] < 0 || m_selectedIndex[i] >= roomListCount
					? null
					: roomList[i].rooms[m_selectedIndex[i]]);
			}
		}

		serializedObject.ApplyModifiedProperties();
	}
}
#endif