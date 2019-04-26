#if UNITY_EDITOR
//////////////////////////////////////////////////////////////////////
//
// Copyright (c) 2014 Audiokinetic Inc. / All Rights Reserved
//
//////////////////////////////////////////////////////////////////////

[UnityEditor.CanEditMultipleObjects]
[UnityEditor.CustomEditor(typeof(AkAmbient))]
public class AkAmbientInspector : AkEventInspector
{
	public enum AttenuationSphereOptions
	{
		Dont_Show,
		Current_Event_Only,
		All_Events
	}

	public static System.Collections.Generic.Dictionary<UnityEngine.Object, AttenuationSphereOptions> attSphereProperties =
		new System.Collections.Generic.Dictionary<UnityEngine.Object, AttenuationSphereOptions>();

	public AttenuationSphereOptions currentAttSphereOp;

	private AkAmbient m_AkAmbient;
	private UnityEditor.SerializedProperty multiPositionTypeProperty;
	private UnityEditor.SerializedProperty largeModePositionArrayProperty;

	private System.Collections.Generic.List<int> triggerList;

	public new void OnEnable()
	{
		base.OnEnable();

		m_AkAmbient = target as AkAmbient;

		multiPositionTypeProperty = serializedObject.FindProperty("multiPositionTypeLabel");
		largeModePositionArrayProperty = serializedObject.FindProperty("LargeModePositions");

		if (!attSphereProperties.ContainsKey(target))
			attSphereProperties.Add(target, AttenuationSphereOptions.Dont_Show);

		currentAttSphereOp = attSphereProperties[target];

		AkWwiseXMLWatcher.Instance.XMLUpdated += PopulateMaxAttenuation;
	}

	public new void OnDisable()
	{
		base.OnDisable();

		DefaultHandles.Hidden = false;

		AkWwiseXMLWatcher.Instance.XMLUpdated -= PopulateMaxAttenuation;
	}

	public override void OnChildInspectorGUI()
	{
		//Save trigger mask to know when it changes
		triggerList = m_AkAmbient.triggerList;

		base.OnChildInspectorGUI();

		if (UnityEngine.Event.current.type == UnityEngine.EventType.ExecuteCommand
			&& UnityEngine.Event.current.commandName == "ObjectSelectorClosed")
		{
			var pickedObject = UnityEditor.EditorGUIUtility.GetObjectPickerObject();
			if (pickedObject != null)
			{
				int insertIndex = largeModePositionArrayProperty.arraySize;
				largeModePositionArrayProperty.InsertArrayElementAtIndex(insertIndex);

				var newElement = largeModePositionArrayProperty.GetArrayElementAtIndex(insertIndex);
				newElement.objectReferenceValue = pickedObject;
				return;
			}
		}

		UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

		using (new UnityEditor.EditorGUILayout.VerticalScope("box"))
		{
			UnityEditor.EditorGUILayout.PropertyField(multiPositionTypeProperty, new UnityEngine.GUIContent("Position Type: ", "Simple Mode: Only one position is used.\nLarge Mode: Children of AkAmbient with AkAmbientLargeModePositioner component will be used as position source for multi-positioning.\nMultiple Position Mode: Every AkAmbient using the same event will be used as position source for multi-positioning."));

			UnityEngine.GUILayout.Space(UnityEditor.EditorGUIUtility.standardVerticalSpacing);

			currentAttSphereOp = (AttenuationSphereOptions) UnityEditor.EditorGUILayout.EnumPopup("Show Attenuation Sphere: ", currentAttSphereOp);
			attSphereProperties[target] = currentAttSphereOp;

			if (multiPositionTypeProperty.intValue == (int)MultiPositionTypeLabel.Large_Mode)
			{
				UnityEngine.GUILayout.BeginHorizontal();
				if (UnityEngine.GUILayout.Button("Add Large Mode position object"))
				{
					int insertIndex = largeModePositionArrayProperty.arraySize;
					largeModePositionArrayProperty.InsertArrayElementAtIndex(insertIndex);

					UnityEngine.GameObject newPoint = new UnityEngine.GameObject(string.Format("AkAmbientPoint{0}", insertIndex));
					UnityEditor.Undo.RegisterCreatedObjectUndo(newPoint, "CreateNewLargeModePositionObject");
					UnityEditor.Undo.AddComponent<AkAmbientLargeModePositioner>(newPoint);
					UnityEditor.Undo.SetTransformParent(newPoint.transform, m_AkAmbient.transform, "CreateNewLargeModePositionObjectSetParent");
					newPoint.transform.position = m_AkAmbient.transform.TransformPoint(UnityEngine.Vector3.zero);
					newPoint.transform.localScale = new UnityEngine.Vector3(1f, 1f, 1f);

					var newElement = largeModePositionArrayProperty.GetArrayElementAtIndex(insertIndex);
					newElement.objectReferenceValue = newPoint.GetComponent<AkAmbientLargeModePositioner>();
				}

				if (UnityEngine.GUILayout.Button("Pick existing position object"))
				{
					int controlID = UnityEngine.GUIUtility.GetControlID(UnityEngine.FocusType.Passive);
					UnityEditor.EditorGUIUtility.ShowObjectPicker<AkAmbientLargeModePositioner>(null, true, string.Empty, controlID);
				}
				UnityEngine.GUILayout.EndHorizontal();

				++UnityEditor.EditorGUI.indentLevel;
				UnityEditor.EditorGUI.BeginChangeCheck();
				for (int i = 0; i < largeModePositionArrayProperty.arraySize; ++i)
				{
					UnityEditor.EditorGUILayout.PropertyField(largeModePositionArrayProperty.GetArrayElementAtIndex(i), true);
				}
				if (UnityEditor.EditorGUI.EndChangeCheck())
				{
					serializedObject.ApplyModifiedProperties();
				}
				--UnityEditor.EditorGUI.indentLevel;
			}
		}

		//Save multi-position type to know if it has changed
		var multiPosType = m_AkAmbient.multiPositionTypeLabel;

		if (m_AkAmbient.multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode)
		{
			UpdateTriggers(multiPosType);
		}
	}

	private void UpdateTriggers(MultiPositionTypeLabel in_multiPosType)
	{
		//if we just switched to MultiPosition_Mode
		if (in_multiPosType != m_AkAmbient.multiPositionTypeLabel)
		{
			//Get all AkAmbients in the scene
			var akAmbients = FindObjectsOfType<AkAmbient>();

			//Find the first AkAmbient that is in multiPosition_Mode and that has the same event as the current AkAmbient
			for (var i = 0; i < akAmbients.Length; i++)
			{
				if (akAmbients[i] != m_AkAmbient &&
				    akAmbients[i].multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode &&
				    akAmbients[i].data.Id == m_AkAmbient.data.Id)
				{
					//if the current AkAmbient doesn't have the same trigger as the others, we ask the user which one he wants to keep
					if (!HasSameTriggers(akAmbients[i].triggerList))
					{
						if (UnityEditor.EditorUtility.DisplayDialog("AkAmbient Trigger Mismatch",
							"All ambients in multi-position mode with the same event must have the same triggers.\n" +
							"Which triggers would you like to keep?", "Current AkAmbient Triggers", "Other AkAmbients Triggers"))
							SetMultiPosTrigger(akAmbients);
						else
							m_AkAmbient.triggerList = akAmbients[i].triggerList;
					}

					break;
				}
			}
		}
		//if the trigger changed or there was an undo/redo operation, we update the triggers of all the AkAmbients in the same group as the current one
		else if (!HasSameTriggers(triggerList) || UnityEngine.Event.current.type == UnityEngine.EventType.ValidateCommand &&
		         UnityEngine.Event.current.commandName == "UndoRedoPerformed")
		{
			var akAmbients = FindObjectsOfType<AkAmbient>();
			SetMultiPosTrigger(akAmbients);
		}
	}

	private bool HasSameTriggers(System.Collections.Generic.List<int> other)
	{
		return other.Count == m_AkAmbient.triggerList.Count &&
		       System.Linq.Enumerable.Count(System.Linq.Enumerable.Except(m_AkAmbient.triggerList, other)) == 0;
	}

	private void SetMultiPosTrigger(AkAmbient[] akAmbients)
	{
		for (var i = 0; i < akAmbients.Length; i++)
		{
			if (akAmbients[i].multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode &&
			    akAmbients[i].data.Id == m_AkAmbient.data.Id)
				akAmbients[i].triggerList = m_AkAmbient.triggerList;
		}
	}

	private void OnSceneGUI()
	{
		RenderAttenuationSpheres();
	}

	public void RenderAttenuationSpheres()
	{
		if (currentAttSphereOp == AttenuationSphereOptions.Dont_Show)
			return;

		if (currentAttSphereOp == AttenuationSphereOptions.Current_Event_Only)
		{
			// Get the max attenuation for the event (if available)
			var radius = AkWwiseProjectInfo.GetData().GetEventMaxAttenuation(m_AkAmbient.data.Id);

			if (m_AkAmbient.multiPositionTypeLabel == MultiPositionTypeLabel.Simple_Mode)
			{
				DrawSphere(m_AkAmbient.gameObject.transform.position, radius);
			}
			else if (m_AkAmbient.multiPositionTypeLabel == MultiPositionTypeLabel.Large_Mode)
			{
				var positionComponents = m_AkAmbient.GetComponentsInChildren<AkAmbientLargeModePositioner>();

				for (int i = 0; i < positionComponents.Length; i++)
				{
					DrawSphere(positionComponents[i].transform.position, radius);
				}
			}
			else
			{
				var akAmbiants = FindObjectsOfType<AkAmbient>();

				for (var i = 0; i < akAmbiants.Length; i++)
				{
					if (akAmbiants[i].multiPositionTypeLabel == MultiPositionTypeLabel.MultiPosition_Mode &&
						akAmbiants[i].data.Id == m_AkAmbient.data.Id)
					{
						DrawSphere(akAmbiants[i].gameObject.transform.position, radius);
					}
				}
			}
		}
		else
		{
			var akAmbiants = FindObjectsOfType<AkAmbient>();

			for (var i = 0; i < akAmbiants.Length; i++)
			{
				// Get the max attenuation for the event (if available)
				var radius = AkWwiseProjectInfo.GetData().GetEventMaxAttenuation(akAmbiants[i].data.Id);

				if (akAmbiants[i].multiPositionTypeLabel == MultiPositionTypeLabel.Large_Mode)
				{
					var positionComponents = m_AkAmbient.GetComponentsInChildren<AkAmbientLargeModePositioner>();

					for (int j = 0; j < positionComponents.Length; j++)
					{
						DrawSphere(positionComponents[j].transform.position, radius);
					}
				}
				else
				{
					DrawSphere(akAmbiants[i].gameObject.transform.position, radius);
				}
			}
		}
	}

    private static UnityEngine.Color SPHERE_COLOR = new UnityEngine.Color(1.0f, 0.0f, 0.0f, 0.1f);

    private void DrawSphere(UnityEngine.Vector3 in_position, float in_radius)
	{
		UnityEditor.Handles.color = SPHERE_COLOR;

		if (UnityEngine.Vector3.SqrMagnitude(
				UnityEditor.SceneView.lastActiveSceneView.camera.transform.position - in_position) > in_radius * in_radius)
		{
#if UNITY_5_6_OR_NEWER
			UnityEditor.Handles.SphereHandleCap(0, in_position, UnityEngine.Quaternion.identity, in_radius * 2.0f,
				UnityEngine.EventType.Repaint);
#else
			UnityEditor.Handles.SphereCap(0, in_position, UnityEngine.Quaternion.identity, in_radius * 2.0f);
#endif
		}
		else
		{
			DrawDiscs(UnityEngine.Vector3.up, UnityEngine.Vector3.down, 6, in_position, in_radius);
		}
	}

	private void DrawDiscs(UnityEngine.Vector3 in_startNormal, UnityEngine.Vector3 in_endNormal, uint in_nbDiscs,
		UnityEngine.Vector3 in_position, float in_radius)
	{
		var f = 1.0f / in_nbDiscs;
		for (var i = 0; i < in_nbDiscs; i++)
		{
			UnityEditor.Handles.DrawWireDisc(in_position, UnityEngine.Vector3.Slerp(in_startNormal, in_endNormal, f * i),
				in_radius);
		}
	}

	public static void PopulateMaxAttenuation()
	{
		UnityEditor.SceneView.RepaintAll();
	}
}
#endif
