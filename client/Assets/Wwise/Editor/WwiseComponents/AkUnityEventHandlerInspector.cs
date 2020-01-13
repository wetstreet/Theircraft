#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

public class AkUnityEventHandlerInspector
{
	private static string[] m_triggerTypeNames;
	private static uint[] m_triggerTypeIDs;
	private static System.Collections.Generic.Dictionary<uint, string> m_triggerTypes;

	///Defines the triggers that make use of useOtherObjectMask
	private static readonly string[] useOtherObjectTriggers =
		{ "AkTriggerEnter", "AkTriggerExit", "AkTriggerCollisionEnter", "AkTriggerCollisionExit" };

	private string m_label = "Trigger On: ";

	private bool m_showUseOtherToggle = true;
	private UnityEditor.SerializedProperty m_triggerList;
	private UnityEditor.SerializedProperty m_useOtherObject;

	public void Init(UnityEditor.SerializedObject in_serializedObject, string in_listName = "triggerList",
		string in_label = "Trigger On: ", bool in_showUseOtherToggle = true)
	{
		m_label = in_label;
		m_showUseOtherToggle = in_showUseOtherToggle;

		m_triggerList = in_serializedObject.FindProperty(in_listName);
		m_useOtherObject = in_serializedObject.FindProperty("useOtherObject");

		//Get the updated list of all triggers
		if (m_triggerTypes == null)
		{
			m_triggerTypes = AkTriggerBase.GetAllDerivedTypes();
			m_triggerTypeNames = new string[m_triggerTypes.Count];
			m_triggerTypes.Values.CopyTo(m_triggerTypeNames, 0);
			m_triggerTypeIDs = new uint[m_triggerTypes.Count];
			m_triggerTypes.Keys.CopyTo(m_triggerTypeIDs, 0);
		}

		//apply the modifications made to the mask property
		in_serializedObject.ApplyModifiedProperties();
	}

	public void OnGUI()
	{
		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			var currentTriggers = GetCurrentTriggers();
			var oldMask = BuildCurrentMaskValue(currentTriggers);

			var newMask = UnityEditor.EditorGUILayout.MaskField(m_label, oldMask, m_triggerTypeNames);

			if (oldMask != newMask)
			{
				currentTriggers.Clear();
				for (var i = 0; i < m_triggerTypeNames.Length; i++)
				{
					var curTriggerID = AkUtilities.ShortIDGenerator.Compute(m_triggerTypeNames[i]);
					if ((newMask & (1 << i)) != 0 && !currentTriggers.Contains(curTriggerID))
						currentTriggers.Add(curTriggerID);
				}

				SaveNewTriggers(currentTriggers);
			}

			if (m_showUseOtherToggle)
			{
				var toggleWasDisplayed = false;

				for (var i = 0; i < m_triggerTypeNames.Length; i++)
				{
					if ((newMask & (1 << i)) != 0 && Contain(useOtherObjectTriggers, m_triggerTypeNames[i]))
					{
						UnityEditor.EditorGUILayout.PropertyField(m_useOtherObject, new UnityEngine.GUIContent("Use Other Object: "));
						toggleWasDisplayed = true;
						break;
					}
				}

				if (!toggleWasDisplayed)
					m_useOtherObject.boolValue = false;
			}
		}

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);
	}

	private System.Collections.Generic.List<uint> GetCurrentTriggers()
	{
		var newList = new System.Collections.Generic.List<uint>();
		for (var i = 0; i < m_triggerList.arraySize; i++)
			newList.Add((uint) m_triggerList.GetArrayElementAtIndex(i).intValue);

		return newList;
	}

	private int GetIdIndex(uint in_ID)
	{
		var index = -1;
		for (var i = 0; i < m_triggerTypeIDs.Length; i++)
		{
			if (m_triggerTypeIDs[i] == in_ID)
			{
				index = i;
				break;
			}
		}

		return index;
	}

	private int BuildCurrentMaskValue(System.Collections.Generic.List<uint> currentTriggers)
	{
		var maskToReturn = 0;
		for (var i = 0; i < currentTriggers.Count; i++)
		{
			var idIndex = GetIdIndex(currentTriggers[i]);
			if (idIndex != -1)
				maskToReturn |= 1 << idIndex;
		}

		return maskToReturn;
	}

	private void SaveNewTriggers(System.Collections.Generic.List<uint> currentTriggers)
	{
		m_triggerList.ClearArray();
		for (var i = 0; i < currentTriggers.Count; i++)
		{
			m_triggerList.InsertArrayElementAtIndex(i);
			m_triggerList.GetArrayElementAtIndex(i).intValue = (int) currentTriggers[i];
		}
	}

	private bool Contain(string[] in_array, string in_value)
	{
		for (var i = 0; i < in_array.Length; i++)
		{
			if (in_array[i].Equals(in_value))
				return true;
		}

		return false;
	}
}
#endif